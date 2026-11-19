using System.Data.Common;

public class Rent
{
    public int Id {get; set;}
    public int PhysicalCopyId {get; set;}
    public int COrderId {get; set;}
    public decimal PriceAtTransaction {get; set;} 
    public DateTime ? ReturnDate {get; set;}
    public DateTime ? ActualReturnDate {get; set;}
    public decimal LateFee {get; set;}
    public bool Invoice {get; set;}
    public bool LateFeePaid {get; set;}
  

    public Rent (int id, decimal priceAtTransaction, DateTime returnDate, int cOrderId, DateTime actualReturnDate, decimal lateFee, bool invoice, bool lateFeePaid, int physicalCopyId)
    {
        Id = id;
        PriceAtTransaction = priceAtTransaction;
        ReturnDate = returnDate;
        COrderId = cOrderId;
        ActualReturnDate = actualReturnDate;
        LateFee = lateFee;
        Invoice = invoice;
        LateFeePaid = lateFeePaid;
        PhysicalCopyId = physicalCopyId;    
    }

}
