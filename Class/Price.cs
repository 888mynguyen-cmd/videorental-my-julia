public class Price
{
    public int Id {get; set;}
    public string Name {get; set;}
    public int Amount {get; set;}  // bytte ut namnet Price till Amount eftersom det inte går att döpa samma namn som klassen

    public Price (int id, string name, int amount)
    {
        Id= id;
        Name = name;
        Amount = amount;
    }
}