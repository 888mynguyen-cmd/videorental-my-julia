using MySqlConnector;
public class RentMenu
{
    public static void ShowRentMenu(MySqlConnection connection)
    {
        // Hyr film - möjlighet att hyra flera filmer
        bool continueRenting = true;

        while (continueRenting)
        {
            Console.WriteLine("------- Hyr film ------");

            Console.Write("Ange kundens ID: ");

            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Ogiltigt kund-ID.");
            }
            else
            {
                Console.Write("Ange filmexemplarets ID: ");

                if (!int.TryParse(Console.ReadLine(), out int physicalCopyId))
                {
                    Console.WriteLine("Ogiltigt filmexemplar-ID.");
                }
                else
                {
                    Console.Write("Har kundens ID kontrollerats? (j/n): ");

                    bool idChecked =
                        Console.ReadLine()?.Trim().ToLower() == "j";

                    Console.Write(
                        "Ange återlämningsdatum (åååå-mm-dd): ");

                    if (!DateTime.TryParseExact(
                        Console.ReadLine(),
                        "yyyy-MM-dd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime returnDate))
                    {
                        Console.WriteLine("Ogiltigt datum.");
                    }
                    else if (returnDate.Date < DateTime.Today)
                    {
                        Console.WriteLine(
                            "Återlämningsdatum får inte vara tidigare än idag.");
                    }
                    else
                    {
                        Console.Write("Bekräfta uthyrningen? (j/n): ");

                        if (Console.ReadLine()?.Trim().ToLower() != "j")
                        {
                            Console.WriteLine("Uthyrningen avbröts.");
                        }
                        else
                        {
                            try
                            {
                                int rentId = RentalService.RentMovie(
                                    connection,
                                    customerId,
                                    physicalCopyId,
                                    idChecked,
                                    returnDate);

                                Console.WriteLine(
                                    $"Uthyrningen registrerades! Hyres-ID: {rentId}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Kunde inte hyra ut filmen: {ex.Message}");
                            }
                        }
                    }
                }
            }

            Console.Write(
                "Vill du hyra ut ytterligare en film? (j/n): ");

            string? answer = Console.ReadLine();

            if (answer?.Trim().ToLower() != "j")
            {
                continueRenting = false;
            }
        }
    }
}