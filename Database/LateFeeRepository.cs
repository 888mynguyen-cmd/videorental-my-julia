using MySqlConnector;

public class LateFeeRepository
{
    public static List<LateRental> GetLateRentals(
        MySqlConnection connection)
    {
        List<LateRental> lateRentals = new();

        const string sql = """
            SELECT
                c.Name,
                c.Phonenumber,
                m.Title,
                pc.Id AS PhysicalCopyId,
                r.ReturnDate,
                rp.Price AS DailyPrice,
                r.PriceAtTransaction
            FROM Rent r
            JOIN COrder co ON r.COrderId = co.Id
            JOIN Customer c ON co.CustomerId = c.Id
            JOIN PhysicalCopy pc ON r.PhysicalCopyId = pc.Id
            JOIN Movie m ON pc.MovieId = m.Id
            JOIN RentalPrice rp ON m.RentalPriceId = rp.Id
            WHERE r.ActualReturnDate IS NULL
              AND r.ReturnDate < @today
            ORDER BY r.ReturnDate
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@today", DateTime.Today);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            DateTime returnDate = reader.GetDateTime("ReturnDate");
            decimal dailyPrice = reader.GetDecimal("DailyPrice");

            lateRentals.Add(new LateRental
            {
                CustomerName = reader.GetString("Name"),
                CustomerPhone = reader.IsDBNull(
                    reader.GetOrdinal("Phonenumber"))
                    ? ""
                    : reader.GetString("Phonenumber"),
                MovieTitle = reader.GetString("Title"),
                PhysicalCopyId = reader.GetInt32("PhysicalCopyId"),
                ReturnDate = returnDate,
                DailyPrice = dailyPrice,
                PriceAtTransaction =
                    reader.GetDecimal("PriceAtTransaction")
            });
        }
        return lateRentals;
    }
}