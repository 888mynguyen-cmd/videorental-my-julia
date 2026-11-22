
using MySqlConnector;

public class ReturnService
{
    public static decimal GetLateFee(
        MySqlConnection connection,
        int physicalCopyId)
    {
        const string sql = """
            SELECT r.ReturnDate, rp.Price
            FROM Rent r
            JOIN PhysicalCopy pc ON r.PhysicalCopyId = pc.Id
            JOIN Movie m ON pc.MovieId = m.Id
            JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
            JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
            WHERE pc.Id = @physicalCopyId
              AND fs.Status = 'Rented'
              AND r.ActualReturnDate IS NULL
            ORDER BY r.Id DESC
            LIMIT 1
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception("Filmen är inte uthyrd eller finns inte.");

        DateTime returnDate = reader.GetDateTime(0);
        decimal dailyPrice = reader.GetDecimal(1);

        int lateDays = Math.Max(
            0,
            (DateTime.Today - returnDate.Date).Days);

        return lateDays * dailyPrice;
    }

    public static ReturnReceipt ReturnMovie(
        MySqlConnection connection,
        int physicalCopyId,
        bool lateFeePaid = false,
        bool invoice = false)
    {
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = """
                SELECT
                    r.Id,
                    r.ReturnDate,
                    c.Name,
                    m.Title,
                    rp.Price
                FROM Rent r
                JOIN COrder co ON r.COrderId = co.Id
                JOIN Customer c ON co.CustomerId = c.Id
                JOIN PhysicalCopy pc ON r.PhysicalCopyId = pc.Id
                JOIN Movie m ON pc.MovieId = m.Id
                JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
                JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
                WHERE pc.Id = @physicalCopyId
                  AND fs.Status = 'Rented'
                  AND r.ActualReturnDate IS NULL
                ORDER BY r.Id DESC
                LIMIT 1
                FOR UPDATE
                """;

            int rentId;
            DateTime returnDate;
            string customerName;
            string movieTitle;
            decimal dailyPrice;
            DateTime actualReturnDate = DateTime.Today;

            using (var command = new MySqlCommand(
                sql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@physicalCopyId", physicalCopyId);

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception(
                        "Filmen är inte uthyrd eller finns inte.");

                rentId = reader.GetInt32(0);
                returnDate = reader.GetDateTime(1);
                customerName = reader.GetString(2);
                movieTitle = reader.GetString(3);
                dailyPrice = reader.GetDecimal(4);
            }

            int lateDays = Math.Max(
                0,
                (actualReturnDate.Date - returnDate.Date).Days);

            decimal lateFee = lateDays * dailyPrice;

            if (lateFee > 0 && lateFeePaid == invoice)
            {
                throw new Exception(
                    "Välj antingen direktbetalning eller faktura.");
            }

            if (lateFee == 0)
            {
                lateFeePaid = false;
                invoice = false;
            }

            const string updateRentSql = """
                UPDATE Rent
                SET ActualReturnDate = @actualReturnDate,
                    LateFee = @lateFee,
                    LateFeePaid = @lateFeePaid,
                    Invoice = @invoice
                WHERE Id = @rentId
                  AND ActualReturnDate IS NULL
                """;

            using (var command = new MySqlCommand(
                updateRentSql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@actualReturnDate", actualReturnDate);
                command.Parameters.AddWithValue("@lateFee", lateFee);
                command.Parameters.AddWithValue("@lateFeePaid", lateFeePaid);
                command.Parameters.AddWithValue("@invoice", invoice);
                command.Parameters.AddWithValue("@rentId", rentId);

                if (command.ExecuteNonQuery() != 1)
                    throw new Exception(
                        "Uthyrningen kunde inte uppdateras.");
            }

            const string statusSql = """
                SELECT Id
                FROM FilmStatus
                WHERE Status = 'Available'
                """;

            int availableStatusId;

            using (var command = new MySqlCommand(
                statusSql, connection, transaction))
            {
                object? result = command.ExecuteScalar();

                if (result == null)
                    throw new Exception("Statusen Available saknas.");

                availableStatusId = Convert.ToInt32(result);
            }

            const string updateCopySql = """
                UPDATE PhysicalCopy
                SET FilmStatusId = @statusId
                WHERE Id = @physicalCopyId
                """;

            using (var command = new MySqlCommand(
                updateCopySql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@statusId", availableStatusId);
                command.Parameters.AddWithValue(
                    "@physicalCopyId", physicalCopyId);

                command.ExecuteNonQuery();
            }

            transaction.Commit();

            return new ReturnReceipt
            {
                CustomerName = customerName,
                MovieTitle = movieTitle,
                PhysicalCopyId = physicalCopyId,
                ReturnDate = returnDate,
                ActualReturnDate = actualReturnDate,
                LateFee = lateFee
            };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
