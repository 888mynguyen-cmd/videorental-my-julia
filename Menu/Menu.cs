
using MySqlConnector;

public class Menu // Här har vi själva menyn
{
    public static void ShowMenu(MySqlConnection connection)
    {
        while (true)
        {
            Console.WriteLine("---------Videoteket--------");
            Console.WriteLine("Vad vill du göra idag?");

            Console.WriteLine("1. Filmer");
            Console.WriteLine("2. Kunder");
            Console.WriteLine("3. Hyr Film");
            Console.WriteLine("4. Återlämning");
            Console.WriteLine("5. Försening");

            Console.WriteLine("Välj alternativ: ");
            int Choice = Convert.ToInt32(Console.ReadLine());

            if (Choice == 1)
            {
                MovieMenu.ShowMovieMenu(connection);     
            }
            else if (Choice == 2)
            {
                CustomerMenu.ShowCustomerMenu(connection);
            }
            else if (Choice == 3)
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
            else if (Choice == 4)
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
            else if (Choice == 5)
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
            else
            {
                Console.WriteLine("Avsluta!");
                break;
            }
        }
    }
}
