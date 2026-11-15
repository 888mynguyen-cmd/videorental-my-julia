
using MySqlConnector;

public class ReturnService
{
    public static decimal GetLateFee(
        MySqlConnection connection,
        int physicalCopyId)
    {
        const string sql = """
            SELECT
                ri.ReturnDate,
                ri.PriceAtTransaction
            FROM RentItem ri
            JOIN Rent r ON ri.RentId = r.Id
            JOIN PhysicalCopy pc ON ri.PhysicalCopyId = pc.Id
            JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
            WHERE pc.Id = @physicalCopyId
              AND fs.Name = 'Rented'
              AND ri.ActualReturnDate IS NULL
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception("Filmen är inte uthyrd eller finns inte.");

        DateTime returnDate = reader.GetDateTime(0);
        decimal rentalPrice = reader.GetDecimal(1);

        int lateDays = Math.Max(
            0,
            (DateTime.Today - returnDate.Date).Days
        );

        return lateDays * rentalPrice;
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
                    ri.RentId,
                    ri.ReturnDate,
                    c.Name,
                    m.Title,
                    ri.PriceAtTransaction
                FROM RentItem ri
                JOIN Rent r ON ri.RentId = r.Id
                JOIN Customer c ON r.CustomerId = c.Id
                JOIN PhysicalCopy pc ON ri.PhysicalCopyId = pc.Id
                JOIN Movie m ON pc.MovieId = m.Id
                JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
                WHERE pc.Id = @physicalCopyId
                  AND fs.Name = 'Rented'
                  AND ri.ActualReturnDate IS NULL
                FOR UPDATE
                """;

            int rentId;
            DateTime returnDate;
            string customerName;
            string movieTitle;
            decimal rentalPrice;
            DateTime actualReturnDate = DateTime.Today;

            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception("Filmen är inte uthyrd eller finns inte.");

                rentId = reader.GetInt32(0);
                returnDate = reader.GetDateTime(1);
                customerName = reader.GetString(2);
                movieTitle = reader.GetString(3);
                rentalPrice = reader.GetDecimal(4);
            }

            int lateDays = Math.Max(
                0,
                (actualReturnDate.Date - returnDate.Date).Days
            );

            decimal lateFee = lateDays * rentalPrice;

            if (lateFee > 0 && lateFeePaid == invoice)
            {
                throw new Exception(
                    "Välj antingen direktbetalning eller faktura."
                );
            }

            if (lateFee == 0)
            {
                lateFeePaid = false;
                invoice = false;
            }

            const string updateRentItemSql = """
                UPDATE RentItem
                SET ActualReturnDate = @actualReturnDate,
                    LateFee = @lateFee,
                    LateFeePaid = @lateFeePaid,
                    Invoice = @invoice
                WHERE RentId = @rentId
                  AND PhysicalCopyId = @physicalCopyId
                  AND ActualReturnDate IS NULL
                """;

            using (var command = new MySqlCommand(
                updateRentItemSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@actualReturnDate", actualReturnDate);
                command.Parameters.AddWithValue("@lateFee", lateFee);
                command.Parameters.AddWithValue("@lateFeePaid", lateFeePaid);
                command.Parameters.AddWithValue("@invoice", invoice);
                command.Parameters.AddWithValue("@rentId", rentId);
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                command.ExecuteNonQuery();
            }

            const string statusSql = """
                SELECT Id
                FROM FilmStatus
                WHERE Name = 'Available'
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
                command.Parameters.AddWithValue("@statusId", availableStatusId);
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

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
