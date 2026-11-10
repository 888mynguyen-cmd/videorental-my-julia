// Sql comman för film
using MySqlConnector;

public class MovieRepository
{    public static List<VHS> ShowMovie(MySqlConnection connection)
    {
        
        using MySqlCommand command = new MySqlCommand("SELECT Id, Title, Description, Incatalog FROM VHS;", connection); // Här skapar vi command för SELECT

        // Kör Sql frågan och läser resultatet
        using MySqlDataReader reader = command.ExecuteReader();

        // Skapar en tom lista där vi ska spara filmerna
        List<VHS> vHs = new List<VHS>();

        // Går igenom varje rad som hämtats från databasen
        while(reader.Read())
        {
            // Hämtar värdena
            int id = reader.GetInt32("Id");
            String title = reader.GetString("Title");
            String description = reader.GetString("Description");
            bool inCatalog = reader.GetBoolean("InCatalog");
            // int priceId = reader.GetInt("PriceId");

            // Skapar ett VHS objekt och lägger till det i listan
            vHs.Add(new VHS(id, title, description, inCatalog));
        }

        // Skickar tillbaka listan
        return vHs; 
    }

    // Lägg till
    public static void AddMovie(MySqlConnection connection, int id, string title, string description, bool inCatalog)
    {   
        int InCatalog = inCatalog ? 1 : 0; // Om inCatalog är true blir värdet 1, annars 0.

        using MySqlCommand command = new MySqlCommand($"INSERT INTO VHS (Id, Title, Description, InCatalog)                              VALUES ({id}, '{title}', '{description}', {InCatalog})", connection);

        command.ExecuteNonQuery();
    }

}