using MySqlConnector;

public class ProductMenu
{
    public static void ShowProduct(MySqlConnection connection)
    {
        while (true)
        {   

            List<Product> products = ProductRepository.ShowProducts(connection);
            foreach(Product product in products)
            {
                Console.WriteLine($" Id: {product.Id} | Product {product.Name} | Price: {product.Price} | Stock: {product.Stock}");
            }
            
            break;
        }
        
    }

    public static void BuyProduct(MySqlConnection connection)
    {
        int cOrderId = COrderRepository.NewOrder(connection);

        bool continueBuying = true;
        while(continueBuying)
        {
            Console.WriteLine("Vilken produkt vill du köpa?");
            int productId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Hur många vill du köpa?");
            int quantity = Convert.ToInt32(Console.ReadLine());


            try
            {
            BuyService.AddProductToOrder(connection, cOrderId, productId, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            Console.Write("Vill du köpa fler produkter? (j/n): ");
            continueBuying = Console.ReadLine()?.Trim().ToLower() == "j";
        }
        ReceiptService receipt = new ReceiptService();
        receipt.LoadOrderFromDatabase(connection, cOrderId);


        decimal total = receipt.GetTotal();
        Console.WriteLine($"Totalt att betala: {total} kr");

        Console.Write("Har kunden betalat? (j/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "j")
        {
            try
            {
                BuyService.PayOrder(connection, cOrderId);
                Console.WriteLine("Betalningen har registrerats!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Betalningen misslyckades: {ex.Message}");
                return;
            }
        }
        else
        {
            Console.WriteLine("Beställningen är inte betald.");
            return;
        }

        Console.Write("Vill kunden ha ett kvitto? (j/n): ");

        if (Console.ReadLine()?.Trim().ToLower() == "j")
        {
            RentMenu.PrintReceipt(receipt);
        }
    }
        
}