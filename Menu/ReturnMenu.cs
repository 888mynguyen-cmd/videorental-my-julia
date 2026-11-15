
using MySqlConnector;

public class ReturnMenu
{
    public static void ShowReturnMenu(MySqlConnection connection)
    {
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
                    decimal lateFee = ReturnService.GetLateFee(
                        connection,
                        physicalCopyId
                    );

                    bool lateFeePaid = false;
                    bool invoice = false;

                    if (lateFee > 0)
                    {
                        Console.WriteLine($"Förseningsavgift: {lateFee} kr");
                        Console.WriteLine("1. Kunden betalar direkt");
                        Console.WriteLine("2. Skicka faktura");

                        string? choice;

                        do
                        {
                            Console.Write("Välj alternativ (1/2): ");
                            choice = Console.ReadLine();

                            if (choice != "1" && choice != "2")
                            {
                                Console.WriteLine("Ogiltigt val.");
                            }

                        } while (choice != "1" && choice != "2");

                        lateFeePaid = choice == "1";
                        invoice = choice == "2";
                    }

                    ReturnReceipt receipt = ReturnService.ReturnMovie(
                        connection,
                        physicalCopyId,
                        lateFeePaid,
                        invoice
                    );

                    Console.WriteLine();
                    Console.WriteLine("Filmen har återlämnats!");

                    Console.Write("Vill kunden ha ett återlämningskvitto? (j/n): ");
                    string? receiptAnswer = Console.ReadLine();

                    if (receiptAnswer?.Trim().ToLower() == "j")
                    {
                        Console.WriteLine();
                        Console.WriteLine("========== ÅTERLÄMNINGSKVITTO ==========");
                        Console.WriteLine($"Kund: {receipt.CustomerName}");
                        Console.WriteLine($"Film: {receipt.MovieTitle}");
                        Console.WriteLine($"Filmexemplar-ID: {receipt.PhysicalCopyId}");
                        Console.WriteLine($"Skulle återlämnas: {receipt.ReturnDate:yyyy-MM-dd}");
                        Console.WriteLine($"Återlämnad: {receipt.ActualReturnDate:yyyy-MM-dd}");
                        Console.WriteLine($"Förseningsavgift: {receipt.LateFee} kr");
                        Console.WriteLine("========================================");
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Kunde inte återlämna film: {ex.Message}");
                }
            }

            Console.Write("Vill du återlämna ytterligare en film? (j/n): ");
            string? answer = Console.ReadLine();

            if (answer?.Trim().ToLower() != "j")
            {
                continueReturning = false;
            }
        }
    }
}
