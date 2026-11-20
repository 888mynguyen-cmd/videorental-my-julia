using System.Globalization;
using MySqlConnector;
public class PriceMenu
{
    public static void ShowPriceMenu(MySqlConnection connection)
    {
        while (true)
        {
            Console.WriteLine("\n--------Prisadministration-----------");
            Console.WriteLine("1. Visa alla priser");
            Console.WriteLine("2. Ändra pris");
            Console.WriteLine("3. Tillbaka");
            Console.WriteLine("Välj:");
            
            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                var prices = PriceRepository.GetAllPrices(connection);
                Console.WriteLine("\n------Alla priser-------");
                foreach(var price in prices)
                {
                    Console.WriteLine($"ID:{price.Id} | {price.Name} | {price.Price} kr");
                }
            }
            else if (choice == "2")
            {
                var prices = PriceRepository.GetAllPrices(connection);
                Console.WriteLine("\n-------Välj pris att ändra------");
                foreach(var price in prices)
                {
                    Console.WriteLine($"ID:{price.Id} | {price.Name} | {price.Price} kr");
                }
                Console.Write("Ange prisets ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Ogiltigt ID");
                    continue;
                }
                if (!prices.Exists(p => p.Id == id))
                {
                    Console.WriteLine("Det finns inget pris med det ID:t");
                    continue;
                }
                Console.Write("Ange det nya priset: ");
                if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal newPrice) || newPrice < 0)
                {
                    Console.WriteLine("Ange ett giltigt pris");
                    continue;
                }

                bool updated = PriceRepository.UpdatePrice(connection, id, newPrice);
                Console.WriteLine(updated? "Priset har ändrats!" : "Priset kunde inte ändras.");
            }
            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Ogiltigt val.");
            }

        }
    }
}

