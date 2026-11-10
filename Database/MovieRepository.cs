// Sql comman för film
using MySqlConnector;

public class MovieRepository
{    public static List<Movie> ShowMovie(MySqlConnection connection)
    {
        
        using MySqlCommand command = new MySqlCommand("SELECT Id, Title, Description, RentalPriceId FROM Movie;", connection); // Här skapar vi command för SELECT

        // Kör Sql frågan och läser resultatet
        using MySqlDataReader reader = command.ExecuteReader();

        // Skapar en tom lista där vi ska spara filmerna
        List<Movie> movie = new List<Movie>();

        // Går igenom varje rad som hämtats från databasen
        while(reader.Read())
        {
            // Hämtar värdena
            int id = reader.GetInt32("Id");
            String title = reader.GetString("Title");
            String description = reader.GetString("Description");
            int rentalPriceId  = reader.GetInt32("RentalPriceId");

            // Skapar ett Movie objekt och lägger till det i listan
            movie.Add(new Movie(id, title, description, rentalPriceId ));
        }

        // Skickar tillbaka listan
        return movie; 
    }

    // Lägg till
    public static void AddMovie(MySqlConnection connection, int id, string title, string description, int rentalPriceId )
    {   
        using MySqlCommand command = new MySqlCommand($"INSERT INTO Movie (Id, Title, Description, RentalPriceId )                  VALUES ({id}, '{title}', '{description}', '{rentalPriceId }')", connection);

        command.ExecuteNonQuery();
    }

    public static void ChangeMovie(MySqlConnection connection, int changeId, string changeTitle, string changeDescription, int changeRentalPriceId)
    {

        using MySqlCommand command = new MySqlCommand($"""
        UPDATE Movie Set Title = '{changeTitle}', Description = '{changeDescription}', RentalPriceId = '{changeRentalPriceId}' WHERE Id = {changeId}
        """, connection);

        command.ExecuteNonQuery();
    }


    public static void RemoveMovie(MySqlConnection connection, int removeMovieId)
    {
        using MySqlCommand command = new MySqlCommand($"""
        DELETE FROM Movie WHERE Id = {removeMovieId} 
        """, connection);

        command.ExecuteNonQuery();
    }


}