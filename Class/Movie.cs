public class Movie
{
    public int Id {get; set;}
    public string Title {get; set;}
    public string Description {get; set;}
    public string Format { get; set; }
    public int RentalPriceId  {get; set;}

    public Movie( int id, string title, string description, string format, int rentalPriceId )
    {
        Id = id;
        Title = title;
        Description = description;
        Format = format;
        RentalPriceId = rentalPriceId;
    }
}