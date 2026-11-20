using System.Data.Common;

public class PhysicalCopy
{
    public int Id {get; set;}
    public string BarCode {get; set;}
    public bool BuyOrRent {get; set;}
    
    public int MovieId {get; set;} 
    public int FilmStatusId {get; set;}

    public string MovieTitle {get; set;} = "";
    public string StatusName {get; set;} = "";

    public PhysicalCopy (int id, string barCode, bool buyOrRent, int movieId, int filmStatusid, string movieTitle, string statusName)
    {
        Id = id;
        BarCode = barCode;
        BuyOrRent = buyOrRent;
        MovieId = movieId;
        FilmStatusId = filmStatusid;
        MovieTitle = movieTitle;
        StatusName = statusName;
    }


}