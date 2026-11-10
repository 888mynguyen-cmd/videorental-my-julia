using System.Data.Common;

public class RentItem
{
    public int Id {get; set;}
    public int PhysicalCopyId {get; set;}
    public int RentId {get; set;}
    public decimal PriceAtTransaction {get; set;} 
    public DateTime ? ReturnDate {get; set;}
    public DateTime ? ActualReturnDate {get; set;}
    public decimal LateFee {get; set;}
    public decimal Price {get; set;}
    public bool Invoice {get; set;}
    public bool LateFeePaid {get; set;}
  

    public RentItem (int id, decimal priceAtTransaction, DateTime returnDate, int rentId, DateTime actualReturnDate, decimal lateFee, decimal price, bool invoice, bool lateFeePaid, int physicalCopyId)
    {
        Id = id;
        PriceAtTransaction = priceAtTransaction;
        ReturnDate = returnDate;
        RentId = rentId;
        ActualReturnDate = actualReturnDate;
        LateFee = lateFee;
        Price = price;
        Invoice = invoice;
        LateFeePaid = lateFeePaid;
        PhysicalCopyId = physicalCopyId;    
    }

}
