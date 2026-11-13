
using MySqlConnector;

public class RentalService
{
    // Kontrollera att kunden finns
    public static bool CustomerExists(MySqlConnection connection, int customerId)
    {
        const string sql = "SELECT COUNT(*) FROM Customer WHERE Id = @customerId";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    // Kontrollera om filmexemplaret är tillgängligt
    public static bool IsCopyAvailable(MySqlConnection connection, int physicalCopyId)
    {
        const string sql = """
            SELECT fs.Name
            FROM PhysicalCopy pc
            JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
            WHERE pc.Id = @physicalCopyId
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

        object? result = command.ExecuteScalar();

        return string.Equals(
            result?.ToString(),
            "Available",
            StringComparison.OrdinalIgnoreCase);
    }

    // Hyr ut ett filmexemplar
    public static int RentMovie(
        MySqlConnection connection,
        int customerId,
        int physicalCopyId,
        bool idChecked,
        DateTime returnDate)
    {
        DateTime rentalDate = DateTime.Today;

        if (returnDate.Date < rentalDate)
            throw new Exception("Återlämningsdatum kan inte vara i det förflutna.");

        // Minst ett dygn debiteras
        int rentalDays = Math.Max(
            1,
            (returnDate.Date - rentalDate).Days
        );

        using var transaction = connection.BeginTransaction();

        try
        {
            // Kontrollera kunden
            using (var command = new MySqlCommand(
                "SELECT COUNT(*) FROM Customer WHERE Id = @customerId",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@customerId", customerId);

                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                    throw new Exception("Kunden finns inte.");
            }

            // Hämta och lås filmexemplaret
            int statusId;
            decimal price;

            const string copySql = """
                SELECT pc.FilmStatusId, rp.Price
                FROM PhysicalCopy pc
                JOIN Movie m ON pc.MovieId = m.Id
                JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
                WHERE pc.Id = @physicalCopyId
                FOR UPDATE
                """;

            using (var command = new MySqlCommand(copySql, connection, transaction))
            {
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception("Filmexemplaret eller priset saknas.");

                if (reader.IsDBNull(0) || reader.IsDBNull(1))
                    throw new Exception("Filmstatus eller pris saknas.");

                statusId = reader.GetInt32(0);
                price = reader.GetDecimal(1);
            }

            // Kontrollera status
            int rentedStatusId;

            const string statusSql = """
                SELECT Id, Name
                FROM FilmStatus
                WHERE Name IN ('Available', 'Rented')
                """;

            using (var command = new MySqlCommand(statusSql, connection, transaction))
            using (var reader = command.ExecuteReader())
            {
                int? availableId = null;
                int? rentedId = null;

                while (reader.Read())
                {
                    string name = reader.GetString(1);

                    if (name.Equals("Available", StringComparison.OrdinalIgnoreCase))
                        availableId = reader.GetInt32(0);

                    if (name.Equals("Rented", StringComparison.OrdinalIgnoreCase))
                        rentedId = reader.GetInt32(0);
                }

                if (availableId == null || rentedId == null)
                    throw new Exception("FilmStatus saknar Available eller Rented.");

                if (statusId != availableId.Value)
                    throw new Exception("Filmexemplaret är inte tillgängligt.");

                rentedStatusId = rentedId.Value;
            }

            // Beräkna total hyreskostnad
            decimal totalPrice = price * rentalDays;

            // Skapa uthyrningen
            const string rentSql = """
                INSERT INTO Rent (Date, Total, Paid, IdChecked, CustomerId)
                VALUES (@date, @total, @paid, @idChecked, @customerId)
                """;

            long rentId;

            using (var command = new MySqlCommand(rentSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@total", totalPrice);
                command.Parameters.AddWithValue("@paid", false);
                command.Parameters.AddWithValue("@idChecked", idChecked);
                command.Parameters.AddWithValue("@customerId", customerId);

                command.ExecuteNonQuery();
                rentId = command.LastInsertedId;
            }

            // Koppla filmexemplaret till uthyrningen
            const string itemSql = """
                INSERT INTO RentItem
                (PhysicalCopyId, RentId, PriceAtTransaction,
                 ReturnDate, ActualReturnDate, LateFee, Invoice, LateFeePaid)
                VALUES
                (@physicalCopyId, @rentId, @price,
                 @returnDate, NULL, 0, false, false)
                """;

            using (var command = new MySqlCommand(itemSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);
                command.Parameters.AddWithValue("@rentId", rentId);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@returnDate", returnDate.Date);

                command.ExecuteNonQuery();
            }

            // Ändra status till uthyrd
            const string updateSql = """
                UPDATE PhysicalCopy
                SET FilmStatusId = @rentedStatusId
                WHERE Id = @physicalCopyId
                """;

            using (var command = new MySqlCommand(updateSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@rentedStatusId", rentedStatusId);
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                command.ExecuteNonQuery();
            }

            transaction.Commit();

            return checked((int)rentId);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
