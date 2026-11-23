
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

    public IReadOnlyList<ReceiptItem> Items => items.AsReadOnly();

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
        List<ReceiptItem> loadedItems =
        ReceiptRepository.GetOrderItems(connection, cOrderId);

        // Ersätt tidigare kvittorader först när allt har hämtats
        items.Clear();
        items.AddRange(loadedItems);
    }

    public decimal GetTotal()
    {
        return items.Sum(item => item.TotalPrice);
    }
}
