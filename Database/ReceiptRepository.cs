using MySqlConnector;

public class ReceiptRepository
{
        // Hämta alla hyrda filmer och köpta produkter från samma order
    public static List<ReceiptItem> GetOrderItems(MySqlConnection connection, int cOrderId)
    {
        const string orderSql = """
            SELECT Id
            FROM COrder
            WHERE Id = @cOrderId
            """;

        using (var command = new MySqlCommand(orderSql, connection))
        {
            command.Parameters.AddWithValue("@cOrderId", cOrderId);

            if (command.ExecuteScalar() == null)
                throw new Exception("Beställningen hittades inte.");
        }

        var loadedItems = new List<ReceiptItem>();

        // Hämta alla hyrda filmer
        const string rentalSql = """
            SELECT m.Title, r.PriceAtTransaction
            FROM Rent r
            JOIN PhysicalCopy pc ON r.PhysicalCopyId = pc.Id
            JOIN Movie m ON pc.MovieId = m.Id
            WHERE r.COrderId = @cOrderId
            """;

        using (var command = new MySqlCommand(rentalSql, connection))
        {
            command.Parameters.AddWithValue("@cOrderId", cOrderId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string title = reader.GetString("Title");
                decimal price = reader.GetDecimal("PriceAtTransaction");

                loadedItems.Add(new ReceiptItem(
                    $"Filmhyra: {title}", 1, price));
            }
        }

        // Hämta alla köpta produkter
        const string productSql = """
            SELECT p.Product, b.Quantity, b.PriceAtTransaction
            FROM Buy b
            JOIN Product p ON b.ProductId = p.Id
            WHERE b.COrderId = @cOrderId
            """;

        using (var command = new MySqlCommand(productSql, connection))
        {
            command.Parameters.AddWithValue("@cOrderId", cOrderId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string productName = reader.GetString("Product");
                int quantity = reader.GetInt32("Quantity");
                decimal price = reader.GetDecimal("PriceAtTransaction");

                loadedItems.Add(new ReceiptItem(
                    productName, quantity, price));
            }
        }
        
        return loadedItems;
    }

}