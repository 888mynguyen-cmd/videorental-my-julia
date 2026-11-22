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
            
        Console.WriteLine("Vilken Product vill du köpa?");
        int ProductChoice = Convert.ToInt32(Console.ReadLine());

        if(ProductChoice == 2)
        {
            
        }

    }
        
}