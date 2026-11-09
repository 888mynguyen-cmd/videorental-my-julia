using System.Data.Common;

public class FilmCode
{
    public int Id {get; set;}
    public string BanCode {get; set;}
    public int VHSId {get; set;} 
    public int FilmStatus {get; set;}

    public FilmCode (int id, string banCode, int vhsId, int filmStatus)
    {

    Id = id;
    BanCode = banCode;
    VHSId = vhsId;
    FilmStatus = filmStatus;
    }


}