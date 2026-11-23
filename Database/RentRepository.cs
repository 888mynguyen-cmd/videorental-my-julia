using MySqlConnector; 

// Stored Procedures
public class RentRepository
{
 // Hämta försenade filmer
    public static List<LateRental> GetLateReturn(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""        
        Call SelectAllLateMovie();
        """, connection);

        using MySqlDataReader reader = command.ExecuteReader();

        List<LateRental> getLateReturns = new List<LateRental>();
        while(reader.Read())
        {
           string title = reader.GetString("Title");
           string name = reader.GetString("Name");
           string phonenumber = reader.GetString("Phonenumber");
           decimal priceAtTransaction = reader.GetDecimal("PriceAtTransaction");
           DateTime returnDate = reader.GetDateTime("ReturnDate");
           int physicalCopyId = reader.GetInt32("PhysicalCopyId");

           getLateReturns.Add(new LateRental(name, phonenumber, title, physicalCopyId, returnDate, priceAtTransaction));
            
        }
        return getLateReturns;
    }
       public static (DateTime ReturnDate, decimal DailyPrice)?GetRentalPrice(
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
            return null;

        DateTime returnDate = reader.GetDateTime(0);
        decimal dailyPrice = reader.GetDecimal(1);

        return (returnDate, dailyPrice);
    }

    public static ReturnInfo? GetReturnInformation(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int physicalCopyId)
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

        using var command = new MySqlCommand(
            sql, connection, transaction);

        command.Parameters.AddWithValue(
            "@physicalCopyId", physicalCopyId);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new ReturnInfo
        {
            RentId = reader.GetInt32(0),
            ReturnDate = reader.GetDateTime(1),
            CustomerName = reader.GetString(2),
            MovieTitle = reader.GetString(3),
            DailyPrice = reader.GetDecimal(4)
        };
    }
    public static void UpdateReturnedRent(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int rentId,
        DateTime actualReturnDate,
        decimal lateFee,
        bool lateFeePaid,
        bool invoice)
    {
        const string sql = """
            UPDATE Rent
            SET ActualReturnDate = @actualReturnDate,
                LateFee = @lateFee,
                LateFeePaid = @lateFeePaid,
                Invoice = @invoice
            WHERE Id = @rentId
            AND ActualReturnDate IS NULL
            """;

        using var command = new MySqlCommand(sql, connection, transaction);

        command.Parameters.AddWithValue("@actualReturnDate", actualReturnDate);
        command.Parameters.AddWithValue("@lateFee", lateFee);
        command.Parameters.AddWithValue("@lateFeePaid", lateFeePaid);
        command.Parameters.AddWithValue("@invoice", invoice);
        command.Parameters.AddWithValue("@rentId", rentId);

        if (command.ExecuteNonQuery() != 1)
            throw new Exception("Uthyrningen kunde inte uppdateras.");
    }
    public static int GetAvailableStatusId(
        MySqlConnection connection,
        MySqlTransaction transaction)
    {
        const string sql = """
            SELECT Id
            FROM FilmStatus
            WHERE Status = 'Available'
            """;

        using var command = new MySqlCommand(
            sql, connection, transaction);

        object? result = command.ExecuteScalar();

        if (result == null)
            throw new Exception("Statusen Available saknas.");

        return Convert.ToInt32(result);
    }
    public static void UpdatePhysicalCopyStatus(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int physicalCopyId,
        int statusId)
    {
        const string sql = """
            UPDATE PhysicalCopy
            SET FilmStatusId = @statusId
            WHERE Id = @physicalCopyId
            """;

        using var command = new MySqlCommand(
            sql, connection, transaction);

        command.Parameters.AddWithValue("@statusId", statusId);
        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);
        if (command.ExecuteNonQuery() != 1)
            throw new Exception("Filmkopian kunde inte uppdateras.");
    }
    public static bool CustomerExists(
        MySqlConnection connection,
        int customerId)
    {
        const string sql =
            "SELECT COUNT(*) FROM Customer WHERE Id = @customerId";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customerId", customerId);

        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }
    public static bool CustomerExists(
        MySqlConnection connection, MySqlTransaction transaction,
        int customerId)
    {
        const string sql =
            "SELECT COUNT(*) FROM Customer WHERE Id = @customerId";

        using var command = new MySqlCommand(sql, connection, transaction);
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
    public static (int CustomerId, bool Paid)? GetOrderInfo(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int cOrderId)
    {
        const string sql = """
            SELECT CustomerId, Paid
            FROM COrder
            WHERE Id = @cOrderId
            FOR UPDATE
            """;

        using var command = new MySqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@cOrderId", cOrderId);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return (
            reader.GetInt32("CustomerId"),
            reader.GetBoolean("Paid")
        );
    }
    public static (int StatusId, bool BuyOrRent, decimal DailyPrice)?
        GetRentalCopyInfo(
            MySqlConnection connection,
            MySqlTransaction transaction,
            int physicalCopyId)
        {
            const string sql = """
                SELECT pc.FilmStatusId,
                    pc.BuyOrRent,
                    rp.Price
                FROM PhysicalCopy pc
                JOIN Movie m ON pc.MovieId = m.Id
                JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
                WHERE pc.Id = @physicalCopyId
                FOR UPDATE
                """;

            using var command = new MySqlCommand(sql, connection, transaction);

            command.Parameters.AddWithValue(
                "@physicalCopyId", physicalCopyId);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            if (reader.IsDBNull(0) ||
                reader.IsDBNull(1) ||
                reader.IsDBNull(2))
            {
                throw new Exception("Filmstatus eller pris saknas.");
            }

            return (
                reader.GetInt32(0),
                reader.GetBoolean(1),
                reader.GetDecimal(2)
            );
        }
    public static (int AvailableStatusId, int RentedStatusId)?
        GetRentalStatuses(
            MySqlConnection connection,
            MySqlTransaction transaction)
    {
        const string sql = """
            SELECT Id, Status
            FROM FilmStatus
            WHERE Status IN ('Available', 'Rented')
            """;

        using var command = new MySqlCommand(sql, connection, transaction);
        using var reader = command.ExecuteReader();

        int? availableStatusId = null;
        int? rentedStatusId = null;

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
                rentedStatusId = reader.GetInt32(0);
            }
        }

        if (!availableStatusId.HasValue ||
            !rentedStatusId.HasValue)
        {
            return null;
        }

        return (availableStatusId.Value, rentedStatusId.Value);
    }
    public static int CreateOrder(
    MySqlConnection connection,
    MySqlTransaction transaction,
    bool idChecked,
    int customerId)
    {
        const string sql = """
            INSERT INTO COrder
            (Date, Total, Paid, IdChecked, CustomerId)
            VALUES
            (@date, 0, false, @idChecked, @customerId)
            """;

        using var command = new MySqlCommand(sql, connection, transaction);

        command.Parameters.AddWithValue("@date", DateTime.Now);
        command.Parameters.AddWithValue("@idChecked", idChecked);
        command.Parameters.AddWithValue("@customerId", customerId);

        command.ExecuteNonQuery();

        return checked((int)command.LastInsertedId);
    }
    public static void CreateRent(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int physicalCopyId,
        int orderId,
        decimal rentalTotal,
        DateTime returnDate)
    {
        const string sql = """
            INSERT INTO Rent
            (PhysicalCopyId, COrderId, PriceAtTransaction,
            ReturnDate, ActualReturnDate, LateFee,
            Invoice, LateFeePaid)
            VALUES
            (@physicalCopyId, @cOrderId, @price,
            @returnDate, NULL, 0, false, false)
            """;

        using var command = new MySqlCommand(sql, connection, transaction);

        command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);
        command.Parameters.AddWithValue("@cOrderId", orderId);
        command.Parameters.AddWithValue("@price", rentalTotal);
        command.Parameters.AddWithValue("@returnDate", returnDate.Date);

        command.ExecuteNonQuery();
    }
    public static void UpdateOrderTotal(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int orderId,
        decimal rentalTotal,
        bool idChecked)
    {
        const string sql = """
            UPDATE COrder
            SET Total = Total + @rentalTotal,
                IdChecked = IdChecked OR @idChecked
            WHERE Id = @cOrderId
            """;

        using var command = new MySqlCommand(sql, connection, transaction);

        command.Parameters.AddWithValue("@rentalTotal", rentalTotal);
        command.Parameters.AddWithValue("@idChecked", idChecked);
        command.Parameters.AddWithValue("@cOrderId", orderId);

        command.ExecuteNonQuery();
    }
    public static void UpdateCopyStatus(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int physicalCopyId,
        int rentedStatusId)
    {
        const string sql = """
            UPDATE PhysicalCopy
            SET FilmStatusId = @rentedStatusId
            WHERE Id = @physicalCopyId
            """;

        using var command = new MySqlCommand(sql, connection, transaction);

        command.Parameters.AddWithValue(
            "@rentedStatusId", rentedStatusId);

        command.Parameters.AddWithValue(
            "@physicalCopyId", physicalCopyId);

        command.ExecuteNonQuery();
    }
}    