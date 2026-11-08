using System.Data.Common;

public class BuyRent
{
    public int Id {get; set;}
    public DateTime Date {get; set;}
    public bool IdChecked {get; set;}
    public DateTime ReturnDate {get; set;}
    public DateTime ? ActualReturnDate {get; set;}
    public decimal LateFee {get; set;}
    public decimal Price {get; set;}
    public bool Invoice {get; set;}
    public bool Paid {get; set;}
    public bool LateFeePaid {get; set;}
    public int CustomerId {get; set;}
    public int FilmCodeId {get; set;}

    public BuyRent (int id, DateTime date, bool idChecked, DateTime actualReturnDate, decimal lateFee, decimal price, bool invoice, bool paid, bool lateFeePaid, int customerId, int filmCodeId)
    {
        Id = id;
        Date = date;
        IdChecked = idChecked;
        ActualReturnDate = actualReturnDate;
        LateFee = lateFee;
        Price = price;
        Invoice = invoice;
        Paid = paid;
        LateFeePaid = lateFeePaid;
        CustomerId = customerId;
        FilmCodeId = filmCodeId;    
    }

}
