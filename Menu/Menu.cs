
using MySqlConnector;

public class Menu
{
    public static void ShowMenu(MySqlConnection connection)
    {
        while (true)
        {
            Console.WriteLine("---------Videoteket--------");
            Console.WriteLine("Vad vill du göra idag?");

            Console.WriteLine("1. Filmer");
            Console.WriteLine("2. Kunder");
            Console.WriteLine("3. Hyr Film");
            Console.WriteLine("4. Köp");
            Console.WriteLine("5. Återlämning");
            Console.WriteLine("6. Försening");

            Console.WriteLine("Välj alternativ: ");
            int Choice = Convert.ToInt32(Console.ReadLine());

            if (Choice == 1)
            {
                MovieMenu.ShowMovieMenu(connection);     
            }
            else if (Choice == 2)
            {
                CustomerMenu.ShowCustomerMenu(connection);
            }
            else if (Choice == 3)
            {
                RentMenu.ShowRentMenu(connection);
            }
            else if (Choice == 4)
            {
                BuyProdcutMenu.BuyMenu(connection);
            }
            else if (Choice == 5)
            {
                ReturnMenu.ShowReturnMenu(connection);
            }
            else if (Choice == 6)
            {
                LateFeeMenu.ShowLateFeeMenu(connection);
            }
            else
            {
                Console.WriteLine("Avsluta!");
                break;
            }
        }
    }
}
