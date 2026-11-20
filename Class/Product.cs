public class Product
{
    public int Id { get; set;}
    public string Name { get; set;}
    public decimal Price { get; set;}
    public int Stock { get; set;}

    public Product(int id, string product, decimal price, int stock)
    {
        Id = id;
        Name = product; // Name pga av propertyn men sen vid hämta product så "Product"
        Price = price;
        Stock = stock;

    }
}

