public class FilmStatus
{
    public int Id { get; set;}
    public string Name {get; set;}

    public FilmStatus (int id, string name)
    {
        Id = id;
        Name = name;
    }
}