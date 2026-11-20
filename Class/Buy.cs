public class Buy
{
    public int Id { get; set;}
    public int Quantity { get; set;}
    public decimal PriceAtTransaction { get; set;}
    public int COrderId { get; set;}
    public int ProductId { get; set;}

    public Buy(int id, int quantity, decimal priceAtTransaction, int cOrderId, int productId)
    {
        Id = id;
        Quantity = quantity;
        PriceAtTransaction = priceAtTransaction;
        COrderId = cOrderId;
        ProductId = productId;
    }
}