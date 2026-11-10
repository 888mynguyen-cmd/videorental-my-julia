using System.Data.Common;

public class FilmCode
{
    public int Id {get; set;}
    public string BarCode {get; set;}
    
    public int MovieId {get; set;} 
    public int FilmStatus {get; set;}

    public FilmCode (int id, string barCode, int movieId, int filmStatus)
    {

    Id = id;
    BarCode = barCode;
    MovieId = movieId;
    FilmStatus = filmStatus;
    }


}