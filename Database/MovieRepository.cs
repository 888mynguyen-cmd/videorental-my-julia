// Sql comman för film
using MySqlConnector;

public class MovieRepository
{    public static List<Movie> ShowMovie(MySqlConnection connection)
    {
        
        using MySqlCommand command = new MySqlCommand("SELECT Id, Title, Description, Format, RentalPriceId FROM Movie;", connection); // Här skapar vi command för SELECT

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
            string format = reader.GetString("Format");
            int rentalPriceId  = reader.GetInt32("RentalPriceId");

            // Skapar ett Movie objekt och lägger till det i listan
            movie.Add(new Movie(id, title, description, format, rentalPriceId ));
        }

        // Skickar tillbaka listan
        return movie; 
    }

    // Lägg till
    public static void AddMovie(MySqlConnection connection, string title, string description, string format, int rentalPriceId )
    {   
        using MySqlCommand command = new MySqlCommand("INSERT INTO Movie (Title, Description, Format, RentalPriceId) VALUES (@title, @description, @format, @rentalPriceId)", connection);

        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@description", description);
        command.Parameters.AddWithValue("@format", format);
        command.Parameters.AddWithValue("@rentalPriceId", rentalPriceId);

        command.ExecuteNonQuery();
    }

    public static void ChangeMovie(MySqlConnection connection, int changeId, string changeTitle, string changeDescription, string changeFormat, int changeRentalPriceId)
    {

        using MySqlCommand command = new MySqlCommand("""
        UPDATE Movie 
        Set Title = @changeTitle, Description = @changeDescription,  Format = @changeFormat, RentalPriceId = @changeRentalPriceId
        WHERE Id = @changeId
        """, connection);

        command.Parameters.AddWithValue("@changeId", changeId);
        command.Parameters.AddWithValue("@changeTitle", changeTitle);
        command.Parameters.AddWithValue("@changeDescription", changeDescription);
        command.Parameters.AddWithValue("@changeFormat", changeFormat);
        command.Parameters.AddWithValue("@changeRentalPriceId", changeRentalPriceId);

        command.ExecuteNonQuery();
    }


    public static void RemoveMovie(MySqlConnection connection, int removeMovieId)
    {
        using MySqlCommand command = new MySqlCommand("""
        DELETE FROM Movie
        WHERE Id = @removeMovieId
        """, connection);

        command.Parameters.AddWithValue("@removeMovieId", removeMovieId);

        command.ExecuteNonQuery();
    }


}