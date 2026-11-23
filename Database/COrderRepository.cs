using MySqlConnector;

// Hit får vi flytta resterande COrder Query kod från Services mappen i filerna som är kopplade till en beställning 
public class COrderRepository
{
    public static int NewOrder(MySqlConnection connection)
    {
        using MySqlCommand command = new MySqlCommand("""
        INSERT INTO COrder (Date, Total, Paid, IdChecked, CustomerId)
        VALUES (NOW(), 0, false, false, NULL);
        """, connection);

        command.ExecuteNonQuery();
        return (int)command.LastInsertedId; // Hämtar och returnerar id på den nyaste skapade beställningen 
    }
    
}