
using MySqlConnector;

public class LateFeeService
{
    private const decimal LateFeePerDay = 10;

    public static void ShowLateRentals(MySqlConnection connection)
    {
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

        Console.WriteLine("------- Försenade filmer -------");

        bool found = false;

        while (reader.Read())
        {
            found = true;

            string customerName = reader.GetString(0);
            string movieTitle = reader.GetString(1);
            int physicalCopyId = reader.GetInt32(2);
            DateTime returnDate = reader.GetDateTime(3);

            int lateDays = (DateTime.Today - returnDate.Date).Days;
            decimal lateFee = lateDays * LateFeePerDay;

            Console.WriteLine($"Kund: {customerName}");
            Console.WriteLine($"Film: {movieTitle}");
            Console.WriteLine($"Filmexemplar-ID: {physicalCopyId}");
            Console.WriteLine($"Skulle återlämnas: {returnDate:yyyy-MM-dd}");
            Console.WriteLine($"Försenad: {lateDays} dagar");
            Console.WriteLine($"Förseningsavgift hittills: {lateFee} kr");
            Console.WriteLine("------------------------------");
        }

        if (!found)
        {
            Console.WriteLine("Inga försenade filmer hittades.");
        }
    }
}
