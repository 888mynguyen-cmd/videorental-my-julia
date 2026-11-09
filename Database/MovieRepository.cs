// Sql comman för film
using MySqlConnector;

public class MovieRepository
{    public static List<VHS> ShowMovie(MySqlConnection connection)
    {
        
        using MySqlCommand command = new MySqlCommand("SELECT Id, Title, Description, Incatalog, RentalPriceId, BuyPriceId FROM VHS;", connection); // Här skapar vi command för SELECT

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
            int RentalpriceId = reader.GetInt32(reader.GetOrdinal("RentalPriceId"));
            int BuyPriceId = reader.GetInt32(reader.GetOrdinal("BuyPriceId"));

            // Skapar ett VHS objekt och lägger till det i listan
            vHs.Add(new VHS(id, title, description, inCatalog, RentalPriceId, BuyPriceId));
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

    public static void ChangeMovie(MySqlConnection connection, int changeId, string changeTitle, string changeDescription, bool changeInCatalog)
    {
        int ChangeInCatalog = changeInCatalog ? 1 : 0; // Om inCatalog är true blir värdet 1, annars 0.

        using MySqlCommand command = new MySqlCommand($"""
        UPDATE VHS Set Title = '{changeTitle}', Description = '{changeDescription}', InCatalog = {ChangeInCatalog} WHERE Id = {changeId}
        """, connection);

        command.ExecuteNonQuery();
    }


    public static void RemoveMovie(MySqlConnection connection, int removeMovieId)
    {
        using MySqlCommand command = new MySqlCommand($"""
        DELETE FROM VHS WHERE Id = {removeMovieId} 
        """, connection);

        command.ExecuteNonQuery();
    }


}