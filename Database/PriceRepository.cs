using MySqlConnector;
public class PriceRepository
{
    public static List<(int Id, string Name, decimal Price)> GetAllPrices(MySqlConnection connection)
    {
    

        using MySqlCommand command = new MySqlCommand("""
        SELECT Id, Name, Price
        FROM RentalPrice 
        """, connection);

        List<(int Id, string Name, decimal Price)> prices = new();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            prices.Add((
                reader.GetInt32("Id"),
                reader.GetString("Name"),
                reader.GetDecimal("Price")
            ));
        }
        return prices;

    }
    public static bool UpdatePrice(
        MySqlConnection connection, int id, decimal price)
    {
        using MySqlCommand command = new MySqlCommand("""
        Update RentalPrice
        SET Price = @Price
        Where Id = @Id
        """, connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Price", price);

        return command.ExecuteNonQuery() > 0;

    }
}