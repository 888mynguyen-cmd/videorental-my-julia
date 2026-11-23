
using MySqlConnector;

public class BuyService
{
    public static void AddProductToOrder(
        MySqlConnection connection,
        int cOrderId,
        int productId,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception("Antalet måste vara större än 0.");
        }

        BuyRepository.AddProductToOrder(
            connection, cOrderId, productId, quantity);
    }

    public static void BuyProducts(MySqlConnection connection, int productId, int quantity)
    {
        if(quantity <= 0)
        {
            throw new Exception("Antalet måste vara större än 0 ");
        }
            int cOrderId = COrderRepository.NewOrder(connection); // Skapa en beställning och ge Id
            AddProductToOrder(connection, cOrderId, productId, quantity); // Lägger till produkterna i beställning
    }

    public static void PayOrder(MySqlConnection connection, int cOrderId) // Ska kontrollera att beställningen innehåller produkter innan den genomförs
    {
        ReceiptService receipt = new ReceiptService();
        receipt.LoadOrderFromDatabase(connection, cOrderId);

        if (receipt.Items.Count == 0)
        {
            throw new Exception("Beställningen är tom och kan inte betalas.");
        }

        COrderRepository.PayOrder(connection, cOrderId);
    }
}
