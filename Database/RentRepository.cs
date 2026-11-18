using MySqlConnector; 

public class RentRepository
{
 // Hämta försenade filmer
    public static void GetLateReturn(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""        
        Call SelectAllLateMovie();
        """, connection);

        using MySqlDataReader reader = command.ExecuteReader();

        List<ReturnMenu> getLateReturns = new List<ReturnMenu>();
        while(reader.Read())
        {
           string Title = reader.GetString("Title");
           string Name = reader.GetString("Name");
           string Phonenumber = reader.GetString("Phonenumber");
           DateTime ReturnDate = reader.GetDateTime("ReturnDate");
           int PhysicalCopyId = reader.GetInt32("PhysicalCopyId");
            
        }
    }
}    