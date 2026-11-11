using MySqlConnector;

public class PhysicalCopyRepository
{
    // Visa alla PhysicalCopys
    public static List<PhysicalCopy> ShowPhysicalCopy(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""
        SELECT PhysicalCopy.Id, PhysicalCopy.BarCode, PhysicalCopy.MovieId, Movie.Title, PhysicalCopy.FilmStatusId, FilmStatus.Name
        FROM PhysicalCopy                 
        JOIN Movie ON PhysicalCopy.MovieId = Movie.Id      
        JOIN FilmStatus ON PhysicalCopy.FilmStatusId = FilmStatus.Id;
        """, connection);

        using MySqlDataReader reader = command.ExecuteReader();

        List<PhysicalCopy> physicalCopies = new List<PhysicalCopy>();

        while(reader.Read())
        {
            int Id = reader.GetInt32("Id");
            string BarCode = reader.GetString("BarCode");
            int MovieId = reader.GetInt32("MovieId");
            int FilmStatusId = reader.GetInt32("FilmStatusId");
            string MovieTitle = reader.GetString("Title");
            string StatusName = reader.GetString("Name");

            physicalCopies.Add(new PhysicalCopy(Id, BarCode, MovieId, FilmStatusId, MovieTitle, StatusName));
        }
        return physicalCopies;
    }

    // Lägg till PhysicalCopy

    public static void AddPhysicalCopy(MySqlConnection connection, string barCode, int movieId, int filmStatusId)
    {
        using MySqlCommand command = new MySqlCommand("INSERT INTO PhysicalCopy (BarCode, MovieId, FilmStatusId)                         VALUES (@barcode, @movieId, @filmStatusId);", connection);
        
        command.Parameters.AddWithValue("@barcode", barCode);
        command.Parameters.AddWithValue("@movieId", movieId);
        command.Parameters.AddWithValue("@filmStatusId",filmStatusId);

        command.ExecuteNonQuery();
    }
    
    
}