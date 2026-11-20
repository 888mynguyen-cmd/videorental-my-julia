public class LateRental
{
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string MovieTitle { get; set; } = "";
    public int PhysicalCopyId { get; set; }
    public DateTime ReturnDate { get; set; }
    public int LateDays { get; set; }
    public decimal LateFee { get; set; }

    public decimal PriceAtTransaction { get; set; }


        public LateRental()
        {
        }

    public LateRental(string customerName, string customerPhone, string movieTitle, int physicalCopyId, DateTime returnDate, decimal priceAtTransaction)
    {
        CustomerName = customerName;
        CustomerPhone = customerPhone;
        MovieTitle = movieTitle;
        PhysicalCopyId = physicalCopyId;
        ReturnDate = returnDate;
        PriceAtTransaction = priceAtTransaction;
    }
}