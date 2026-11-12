using MySqlConnector;

public class LateFeeMenu
{
    public static void ShowLateFeeMenu(MySqlConnection connection)
    {
        // Försening
        try
        {
            LateFeeService.ShowLateRentals(connection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Kunde inte visa förseningar: {ex.Message}");
        }
    }
}