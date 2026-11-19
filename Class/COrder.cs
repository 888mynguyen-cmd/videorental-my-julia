class COrder
{
    public int Id {get;set;}
    public DateTime Date { get;set;}
    public decimal Total {get;set;}
    public bool Paid {get; set;}
    public bool IdChecked {get; set;}
    public int CustomerId {get; set;}

    public COrder (int id, DateTime date, decimal total, bool paid, bool idchecked, int customerId)
    {
        Id = id;
        Date = date;
        Total = total;
        Paid = paid;
        IdChecked = idchecked;
        CustomerId = customerId;

    }
}





