using MySqlConnector;

public class BuyProdcutMenu
{
    public static void BuyMenu(MySqlConnection connection)
    {
        while (true)
        {
            Console.WriteLine("---------Köp--------");
            Console.WriteLine("1. Visa alla produkter");
            Console.WriteLine("2. Köp produkter");
            Console.WriteLine("3. Tillbaka till huvudmenyn");

            Console.WriteLine("Vad vill du göra?");
            
            int buyChoice = Convert.ToInt32(Console.ReadLine());
            if(buyChoice == 1)
            {
                ProductMenu.ShowProduct(connection);
                break;
            }

            else if (buyChoice == 2)
            {
                ProductMenu.BuyProduct(connection);
                break;
            }

            else
            {
                Console.WriteLine("Avsluta!");
                break;
            }
        }
    }
}