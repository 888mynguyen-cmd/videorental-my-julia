using MySqlConnector;

public class ReturnMenu
{
    public static void ShowReturnMenu(MySqlConnection connection)
    {   
        // Återlämning
        Console.WriteLine("------- Återlämning ------");

        bool continueReturning = true;

        while (continueReturning)
        {
            Console.Write("Ange filmexemplarets ID: ");

            if (!int.TryParse(Console.ReadLine(), out int physicalCopyId))
            {
                Console.WriteLine("Ogiltigt filmexemplar-ID.");
            }
            else
            {
                try
                {
                    ReturnService.ReturnMovie(connection, physicalCopyId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Kunde inte återlämna Film: {ex.Message}");
                }
            }

            Console.Write(
                "Vill du återlämna ytterligare en film? (j/n): ");

            string? answer = Console.ReadLine();

            if (answer?.ToLower() != "j")
            {
                continueReturning = false;
            }
        }
        
    }
}