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
        while (true)
        {
            Console.WriteLine("Vilken produkt vill du köpa?");
            int productId = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Hur många vill du köpa?");
            int quantity = Convert.ToInt32(Console.ReadLine());

        }
        

    }
        
}