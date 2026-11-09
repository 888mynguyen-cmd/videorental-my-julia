using System.Data.Common;

public class BuyRentItem
{
    public int Id {get; set;}
    public int FilmCodeId {get; set;}
    public int BuyRentId {get; set;}
    public decimal PriceAtTransaction {get; set;} 
    public DateTime ? ReturnDate {get; set;}
    public bool IsRental {get; set;}
    public DateTime ? ActualReturnDate {get; set;}
    public decimal LateFee {get; set;}
    public decimal Price {get; set;}
    public bool Invoice {get; set;}
    public bool LateFeePaid {get; set;}
  

    public BuyRentItem (int id, decimal priceAtTransaction, DateTime returnDate, bool isRenal, DateTime actualReturnDate, decimal lateFee, decimal price, bool invoice, bool lateFeePaid, int buyRentId, int filmCodeId)
    {
        Id = id;
        PriceAtTransaction = priceAtTransaction;
        ReturnDate = returnDate;
        IsRental = isRenal;
        ActualReturnDate = actualReturnDate;
        LateFee = lateFee;
        Price = price;
        Invoice = invoice;
        LateFeePaid = lateFeePaid;
        BuyRentId = buyRentId;
        FilmCodeId = filmCodeId;    
    }

}
