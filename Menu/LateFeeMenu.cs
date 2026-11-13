
using MySqlConnector;

public class LateFeeMenu
{
    public static void ShowLateFeeMenu(MySqlConnection connection)
    {
        try
        {
            var lateRentals = LateFeeService.GetLateRentals(connection);

            Console.WriteLine("------- Försenade filmer -------");

            if (lateRentals.Count == 0)
            {
                Console.WriteLine("Inga försenade filmer hittades.");
                return;
            }

            foreach (var rental in lateRentals)
            {
                Console.WriteLine($"Kund: {rental.CustomerName}");
                Console.WriteLine($"Film: {rental.MovieTitle}");
                Console.WriteLine($"Filmexemplar-ID: {rental.PhysicalCopyId}");
                Console.WriteLine($"Skulle återlämnas: {rental.ReturnDate:yyyy-MM-dd}");
                Console.WriteLine($"Försenad: {rental.LateDays} dagar");
                Console.WriteLine($"Förseningsavgift hittills: {rental.LateFee} kr");
                Console.WriteLine("------------------------------");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kunde inte visa förseningar: {ex.Message}");
        }
    }
}
