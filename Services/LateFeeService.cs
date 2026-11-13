
using MySqlConnector;

public class LateRental
{
    public string CustomerName { get; set; } = "";
    public string MovieTitle { get; set; } = "";
    public int PhysicalCopyId { get; set; }
    public DateTime ReturnDate { get; set; }
    public int LateDays { get; set; }
    public decimal LateFee { get; set; }
}

public class LateFeeService
{
    private const decimal LateFeePerDay = 10;

    public static List<LateRental> GetLateRentals(MySqlConnection connection)
    {
        List<LateRental> lateRentals = new();

        const string sql = """
            SELECT
                c.Name,
                m.Title,
                pc.Id,
                ri.ReturnDate
            FROM RentItem ri
            JOIN Rent r ON ri.RentId = r.Id
            JOIN Customer c ON r.CustomerId = c.Id
            JOIN PhysicalCopy pc ON ri.PhysicalCopyId = pc.Id
            JOIN Movie m ON pc.MovieId = m.Id
            WHERE ri.ActualReturnDate IS NULL
              AND ri.ReturnDate < @today
            ORDER BY ri.ReturnDate
            """;

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@today", DateTime.Today);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            DateTime returnDate = reader.GetDateTime(3);
            int lateDays = (DateTime.Today - returnDate.Date).Days;

            lateRentals.Add(new LateRental
            {
                CustomerName = reader.GetString(0),
                MovieTitle = reader.GetString(1),
                PhysicalCopyId = reader.GetInt32(2),
                ReturnDate = returnDate,
                LateDays = lateDays,
                LateFee = lateDays * LateFeePerDay
            });
        }

        return lateRentals;
    }
}
