
public class ReturnReceipt
{
    public string CustomerName { get; set; } = "";
    public string MovieTitle { get; set; } = "";
    public int PhysicalCopyId { get; set; }
    public DateTime ReturnDate { get; set; }
    public DateTime ActualReturnDate { get; set; }
    public decimal LateFee { get; set; }
}
