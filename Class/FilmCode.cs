using System.Data.Common;

public class FilmCode
{
    public int Id {get; set;}
    public string BanCode {get; set;}
    public int MovieId {get; set;} 
    public int FilmStatus {get; set;}

    public FilmCode (int id, string banCode, int movieId, int filmStatus)
    {

    Id = id;
    BanCode = banCode;
    MovieId = movieId;
    FilmStatus = filmStatus;
    }


}