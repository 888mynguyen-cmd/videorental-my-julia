using MySqlConnector;

public class LateFeeService
{
    public static List<LateRental> GetLateRentals(MySqlConnection connection)
    {
        // Använd om du vill hämta alla försenade filmer via vår stored procedure SelectAllLateMovie (finns i RentRepository)
        // List<LateRental> lateRentals = RentRepository.GetLateReturn(connection);
        List<LateRental> lateRentals = new(); 

        const string sql = """
            SELECT
                c.Name,
                c.Phonenumber,
                m.Title,
                pc.Id,
                ri.ReturnDate,
                ri.PriceAtTransaction
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
            DateTime returnDate = reader.GetDateTime(4);
            decimal rentalPrice = reader.GetDecimal(5);

            int lateDays = (DateTime.Today - returnDate.Date).Days;

            lateRentals.Add(new LateRental
            {
                CustomerName = reader.GetString(0),
                CustomerPhone = reader.IsDBNull(1) ? "" : reader.GetString(1),
                MovieTitle = reader.GetString(2),
                PhysicalCopyId = reader.GetInt32(3),
                ReturnDate = returnDate,
                LateDays = lateDays,
                LateFee = lateDays * rentalPrice
            });
        }

        return lateRentals;
    }
}
