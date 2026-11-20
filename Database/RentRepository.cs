using MySqlConnector; 

// Stored Procedures
public class RentRepository
{
 // Hämta försenade filmer
    public static List<LateRental> GetLateReturn(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""        
        Call SelectAllLateMovie();
        """, connection);

        using MySqlDataReader reader = command.ExecuteReader();

        List<LateRental> getLateReturns = new List<LateRental>();
        while(reader.Read())
        {
           string Title = reader.GetString("Title");
           string Name = reader.GetString("Name");
           string Phonenumber = reader.GetString("Phonenumber");
           decimal PriceAtTransaction = reader.GetDecimal("PriceAtTransaction");
           DateTime ReturnDate = reader.GetDateTime("ReturnDate");
           int PhysicalCopyId = reader.GetInt32("PhysicalCopyId");

           getLateReturns.Add(new LateRental(Name, Phonenumber, Title, PhysicalCopyId, ReturnDate, PriceAtTransaction));
            
        }
        return getLateReturns;
    }
}    