
using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;

public class ReceiptItem
{
    public string Name { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public decimal TotalPrice => Quantity * UnitPrice;

    public ReceiptItem(string name, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Namn saknas.");

        if (quantity <= 0)
            throw new ArgumentException("Antal måste vara större än 0.");

        if (unitPrice < 0)
            throw new ArgumentException("Priset får inte vara negativt.");

        Name = name;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}

public class ReceiptService
{
    private readonly List<ReceiptItem> items = new();

    public void AddRental(string movieTitle, decimal price)
    {
        items.Add(new ReceiptItem(
            $"Filmhyra: {movieTitle}", 1, price));
    }

    public void AddProduct(string productName, int quantity, decimal unitPrice)
    {
        items.Add(new ReceiptItem(
            productName, quantity, unitPrice));
    }

    // Hämta alla hyrda filmer och köpta produkter från samma order
    public void LoadOrderFromDatabase(MySqlConnection connection, int cOrderId)
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

        // Ersätt tidigare kvittorader först när allt har hämtats
        items.Clear();
        items.AddRange(loadedItems);
    }

    public decimal GetTotal()
    {
        return items.Sum(item => item.TotalPrice);
    }

    public void PrintReceipt()
    {
        Console.WriteLine();
        Console.WriteLine("========== VIDEOTEKET ==========");
        Console.WriteLine("             KVITTO");
        Console.WriteLine("--------------------------------");

        foreach (ReceiptItem item in items)
        {
            Console.WriteLine(
                $"{item.Name} | {item.Quantity} st x {item.UnitPrice:0.00} kr = {item.TotalPrice:0.00} kr");
        }

        Console.WriteLine("--------------------------------");
        Console.WriteLine($"TOTALT: {GetTotal():0.00} kr");
        Console.WriteLine("================================");
    }
}
