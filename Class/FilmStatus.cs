public class FilmStatus
{
    public int Id { get; set;}
    public string Status {get; set;}

    public FilmStatus (int id, string status)
    {
        Id = id;
        Status = status;
    }
}