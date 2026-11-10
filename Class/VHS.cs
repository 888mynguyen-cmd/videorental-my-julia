public class Movie
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    // public int PriceId {get; set;}

    public Movie( int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
        // PriceId = priceId; // Behövde kommentera bort tills vi lagt till dom i databasen för kan inte göra referens till den än
    }
}