public class Movie
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public int RentalPriceId  {get; set;}

    public Movie( int id, string title, string description, int rentalPriceId )
    {
        Id = id;
        Title = title;
        Description = description;
        RentalPriceId = rentalPriceId; // Nu kommer den tillbaka men med uppdaterat namn
    }
}