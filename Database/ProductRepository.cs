using MySqlConnector;
// Lägg till menyval för Köp där man kan köpa filmer och snacks och så kan man välja i listan vilka produkter 

class ProductRepository
{
    public static List <Product> ShowProducts(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""
        SELECT Id, Product, Price, Stock FROM Product;
        """, connection);
        
        using MySqlDataReader reader = command.ExecuteReader();

        List<Product> products = new List<Product>();

        while (reader.Read())
        {
            int Id = reader.GetInt32("Id");
            string Name = reader.GetString("Product");
            decimal Price = reader.GetDecimal("Price");
            int Stock = reader.GetInt32("Stock");

            products.Add(new Product(Id, Name, Price, Stock));
        }

        return products;
    }

    // public static List <Product> BuyProducts (MySqlConnection connection)
    // {
    //     using MySqlConnection command = new MySqlConnection("""

    //     """);

    // } 
}