public class VHS
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public bool InCatalog {get; set;}
    public int PriceId {get; set;}

    public VHS( int id, string title, string description, bool inCatalog, int priceId)
    {
        Id = id;
        Title = title;
        Description = description;
        InCatalog = inCatalog;
        PriceId = priceId;
    }
}