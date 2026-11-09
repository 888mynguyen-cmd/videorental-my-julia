// Sql comman för film
using MySqlConnector;

// Lägg till, ta bort, uppdatera film

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
}