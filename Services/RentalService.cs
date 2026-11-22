
using MySqlConnector;

public class RentalService
{
    public static bool CustomerExists(
        MySqlConnection connection, int customerId)
    {
        const string sql =
            "SELECT COUNT(*) FROM Customer WHERE Id = @customerId";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public static bool IsCopyAvailable(
        MySqlConnection connection, int physicalCopyId)
    {
        const string sql = """
            SELECT fs.Status
            FROM PhysicalCopy pc
            JOIN FilmStatus fs ON pc.FilmStatusId = fs.Id
            WHERE pc.Id = @physicalCopyId
              AND pc.BuyOrRent = 0
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

        return string.Equals(
            command.ExecuteScalar()?.ToString(),
            "Available",
            StringComparison.OrdinalIgnoreCase);
    }

    // Skapa en ny order och hyr den första filmen.
    public static int RentMovie(
        MySqlConnection connection,
        int customerId,
        int physicalCopyId,
        bool idChecked,
        DateTime returnDate)
    {
        return RentMovie(
            connection,
            customerId,
            physicalCopyId,
            idChecked,
            returnDate,
            null,
            null);
    }

    // Lägg till en film i en befintlig order,
    // eller skapa en ny order om cOrderId är null.
    // specialPrice = null betyder automatiskt pris.
    public static int RentMovie(
        MySqlConnection connection,
        int customerId,
        int physicalCopyId,
        bool idChecked,
        DateTime returnDate,
        int? cOrderId,
        decimal? specialPrice = null)
    {
        DateTime rentalDate = DateTime.Today;

        if (returnDate.Date < rentalDate)
            throw new Exception(
                "Återlämningsdatum kan inte vara i det förflutna.");

        if (specialPrice.HasValue &&
            (specialPrice.Value < 0 ||
             specialPrice.Value > 99999999.99m))
        {
            throw new Exception("Ogiltigt specialpris.");
        }

        int rentalDays = Math.Max(
            1,
            (returnDate.Date - rentalDate).Days);

        using var transaction = connection.BeginTransaction();

        try
        {
            // Kontrollera kunden.
            using (var command = new MySqlCommand(
                "SELECT COUNT(*) FROM Customer WHERE Id = @customerId",
                connection, transaction))
            {
                command.Parameters.AddWithValue("@customerId", customerId);

                if (Convert.ToInt32(command.ExecuteScalar()) == 0)
                    throw new Exception("Kunden finns inte.");
            }

            // Kontrollera befintlig order.
            if (cOrderId.HasValue)
            {
                const string orderSql = """
                    SELECT CustomerId, Paid
                    FROM COrder
                    WHERE Id = @cOrderId
                    FOR UPDATE
                    """;

                using var command = new MySqlCommand(
                    orderSql, connection, transaction);

                command.Parameters.AddWithValue(
                    "@cOrderId", cOrderId.Value);

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception(
                        "Beställningen finns inte.");

                int orderCustomerId = reader.GetInt32("CustomerId");
                bool isPaid = reader.GetBoolean("Paid");

                if (orderCustomerId != customerId)
                    throw new Exception(
                        "Beställningen tillhör en annan kund.");

                if (isPaid)
                    throw new Exception(
                        "Beställningen är redan betald.");
            }

            // Hämta filmexemplaret och priset.
            int statusId;
            decimal dailyPrice;
            bool buyOrRent;

            const string copySql = """
                SELECT pc.FilmStatusId,
                       pc.BuyOrRent,
                       rp.Price
                FROM PhysicalCopy pc
                JOIN Movie m ON pc.MovieId = m.Id
                JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
                WHERE pc.Id = @physicalCopyId
                FOR UPDATE
                """;

            using (var command = new MySqlCommand(
                copySql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@physicalCopyId", physicalCopyId);

                using var reader = command.ExecuteReader();

                if (!reader.Read())
                    throw new Exception(
                        "Filmexemplaret eller priset saknas.");

                if (reader.IsDBNull(0) ||
                    reader.IsDBNull(1) ||
                    reader.IsDBNull(2))
                {
                    throw new Exception(
                        "Filmstatus eller pris saknas.");
                }

                statusId = reader.GetInt32(0);
                buyOrRent = reader.GetBoolean(1);
                dailyPrice = reader.GetDecimal(2);
            }

            // BuyOrRent = 1 betyder köp.
            if (buyOrRent)
                throw new Exception(
                    "Filmexemplaret är till försäljning och kan inte hyras.");

            int? availableStatusId = null;
            int? rentedId = null;

            const string statusSql = """
                SELECT Id, Status
                FROM FilmStatus
                WHERE Status IN ('Available', 'Rented')
                """;

            using (var command = new MySqlCommand(
                statusSql, connection, transaction))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string status = reader.GetString(1);

                    if (status.Equals(
                        "Available",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        availableStatusId = reader.GetInt32(0);
                    }

                    if (status.Equals(
                        "Rented",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        rentedId = reader.GetInt32(0);
                    }
                }
            }

            if (!availableStatusId.HasValue ||
                !rentedId.HasValue)
            {
                throw new Exception(
                    "FilmStatus saknar Available eller Rented.");
            }

            if (statusId != availableStatusId.Value)
                throw new Exception(
                    "Filmexemplaret är inte tillgängligt.");

            // Använd specialpris om det finns.
            // Annars används automatiskt pris.
            decimal rentalTotal =
                specialPrice ?? (dailyPrice * rentalDays);

            int orderId;

            if (cOrderId.HasValue)
            {
                orderId = cOrderId.Value;
            }
            else
            {
                const string insertOrderSql = """
                    INSERT INTO COrder
                    (Date, Total, Paid, IdChecked, CustomerId)
                    VALUES
                    (@date, 0, false, @idChecked, @customerId)
                    """;

                using var command = new MySqlCommand(
                    insertOrderSql, connection, transaction);

                command.Parameters.AddWithValue(
                    "@date", DateTime.Now);
                command.Parameters.AddWithValue(
                    "@idChecked", idChecked);
                command.Parameters.AddWithValue(
                    "@customerId", customerId);

                command.ExecuteNonQuery();

                orderId = checked((int)command.LastInsertedId);
            }

            // Registrera filmen med överenskommet pris.
            const string insertRentSql = """
                INSERT INTO Rent
                (PhysicalCopyId, COrderId, PriceAtTransaction,
                 ReturnDate, ActualReturnDate, LateFee,
                 Invoice, LateFeePaid)
                VALUES
                (@physicalCopyId, @cOrderId, @price,
                 @returnDate, NULL, 0, false, false)
                """;

            using (var command = new MySqlCommand(
                insertRentSql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@physicalCopyId", physicalCopyId);
                command.Parameters.AddWithValue(
                    "@cOrderId", orderId);
                command.Parameters.AddWithValue(
                    "@price", rentalTotal);
                command.Parameters.AddWithValue(
                    "@returnDate", returnDate.Date);

                command.ExecuteNonQuery();
            }

            // Uppdatera orderns totalbelopp.
            const string updateOrderSql = """
                UPDATE COrder
                SET Total = Total + @rentalTotal,
                    IdChecked = IdChecked OR @idChecked
                WHERE Id = @cOrderId
                """;

            using (var command = new MySqlCommand(
                updateOrderSql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@rentalTotal", rentalTotal);
                command.Parameters.AddWithValue(
                    "@idChecked", idChecked);
                command.Parameters.AddWithValue(
                    "@cOrderId", orderId);

                command.ExecuteNonQuery();
            }

            // Ändra filmens status till Rented.
            const string updateCopySql = """
                UPDATE PhysicalCopy
                SET FilmStatusId = @rentedStatusId
                WHERE Id = @physicalCopyId
                """;

            using (var command = new MySqlCommand(
                updateCopySql, connection, transaction))
            {
                command.Parameters.AddWithValue(
                    "@rentedStatusId", rentedId.Value);
                command.Parameters.AddWithValue(
                    "@physicalCopyId", physicalCopyId);

                command.ExecuteNonQuery();
            }

            transaction.Commit();

            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
