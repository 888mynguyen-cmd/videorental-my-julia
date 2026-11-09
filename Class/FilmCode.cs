using System.Data.Common;

public class FilmCode
{
    public int Id {get; set;}
    public string BarCode {get; set;}
    public bool IsForRental {get ; set;}
    public int VHSId {get; set;} 
    public int FilmStatus {get; set;}

    public FilmCode (int id, string barCode, bool isForRental, int vhsId, int filmStatus)
    {

    Id = id;
    BarCode = barCode;
    IsForRental = isForRental;
    VHSId = vhsId;
    FilmStatus = filmStatus;
    }


}