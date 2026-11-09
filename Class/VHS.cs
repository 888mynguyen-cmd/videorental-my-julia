public class VHS
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public bool InCatalog {get; set;}
    // public int PriceId {get; set;}

    public VHS( int id, string title, string description, bool inCatalog)
    {
        Id = id;
        Title = title;
        Description = description;
        InCatalog = inCatalog;
        // PriceId = priceId; // Behövde kommentera bort tills vi lagt till dom i databasen för kan inte göra referens till den än
    }
}