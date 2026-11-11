
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
                
                while(true)
                {
                    Console.WriteLine("---------Filmer--------");
                    Console.WriteLine("Välj nedan?");

                    Console.WriteLine("1. Visa alla Filmer");
                    Console.WriteLine("2. Lägg till film");
                    Console.WriteLine("3. Ändra film");
                    Console.WriteLine("4. Ta bort");
                    Console.WriteLine("5. Hantera fysiska exempel");
                    Console.WriteLine("6. Gå tillbaka till huvudmenyn");

                    Console.WriteLine("Välj alternativ: ");
                    int FilmChoice = Convert.ToInt32(Console.ReadLine());
                        
                    if (FilmChoice == 1)
                    {
                        // Visa alla filmer
                        List<Movie> movies = MovieRepository.ShowMovie(connection);

                        foreach (Movie movie in movies)
                        {
                            Console.WriteLine(
                                $"{movie.Id} | {movie.Title} | {movie.Description} | PrisId: {movie.RentalPriceId}");
                        }
                    }
                    else if (FilmChoice == 2)
                    {
                        // Lägg till film

                        Console.WriteLine("Lägg till en titel på film ");
                        string Title = Console.ReadLine() ?? "";

                        Console.WriteLine("Lägg till en beskrivning på film ");
                        string Description = Console.ReadLine() ?? "";

                        // 1 = Ny film, 2 = Gammal film
                        Console.WriteLine("Vilket id på priset till filmen?");
                        int RentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");

                        MovieRepository.AddMovie(
                            connection,
                            Title,
                            Description,
                            RentalPriceId);
                    }
                    else if (FilmChoice == 3)
                    {
                        // Ändra film
                        Console.WriteLine("Vilken film vill du ändra?");
                        int changeId = Convert.ToInt32(Console.ReadLine() ?? "");

                        Console.WriteLine("Vad vill du ändra filmens titel till?");
                        string changeTitle = Console.ReadLine() ?? "";

                        Console.WriteLine("Vad vill du ändra filmens beskrivning till?");
                        string changeDescription = Console.ReadLine() ?? "";

                        Console.WriteLine("Vad vill du ändra filmens Pris till?");
                        int changeRentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");

                        MovieRepository.ChangeMovie(
                            connection,
                            changeId,
                            changeTitle,
                            changeDescription,
                            changeRentalPriceId);
                    }
                    else if (FilmChoice == 4)
                    {
                        // Ta bort film
                        Console.WriteLine("Vilken film vill du ta bort?");
                        int removeMovieId = Convert.ToInt32(Console.ReadLine() ?? "");

                        MovieRepository.RemoveMovie(connection, removeMovieId);
                        Console.WriteLine("Filmen har tagits bort!");
                    }
                    else if (FilmChoice == 5)
                    {
                        while(true)
                        {
                            Console.WriteLine("---------Fysiska Exemplar--------");
                            Console.WriteLine("Välj nedan?");

                            Console.WriteLine("1. Visa alla exemplar");
                            Console.WriteLine("2. Lägg till exemplar");
                            Console.WriteLine("3. Ändra exemplarets status");
                            Console.WriteLine("4. Tillbaka till Filmer");

                            Console.WriteLine("Välj alternativ: ");
                            int PhysicalChoice = Convert.ToInt32(Console.ReadLine());

                            if (PhysicalChoice == 1)
                            {
                                List<PhysicalCopy> physicalCopies = PhysicalCopyRepository.ShowPhysicalCopy(connection);

                                foreach (PhysicalCopy physicalCopy in physicalCopies)
                                {
                                    Console.WriteLine(
                                        $"{physicalCopy.Id} | {physicalCopy.BarCode} | {physicalCopy.MovieId} | {physicalCopy.MovieTitle} | {physicalCopy.FilmStatusId} | {physicalCopy.StatusName}");
                                }
                            }

                            else if (PhysicalChoice == 2)
                            {
                                
                            }

                            else if (PhysicalChoice == 3)
                            {
                                
                            }
                            else if (PhysicalChoice == 4)
                            {
                                Console.WriteLine("Tillbaka till Filmer!");
                                break;
                            }
                        }
                    }

                    else if (FilmChoice == 6) 
                    {
                        Console.WriteLine("Tillbaka till Huvudmenyn!");
                        break;
                    }
                }
            }


            else if (Choice == 2)
            {
                // Kundhantering
                CustomerRepository customerRepository = new CustomerRepository();
                int CustomerChoice = 0;

                while (CustomerChoice != 5)
                {
                    Console.WriteLine("---------Kunder--------");
                    Console.WriteLine("Välj nedan?");
                    Console.WriteLine("1. Visa alla kunder");
                    Console.WriteLine("2. Lägg till kund");
                    Console.WriteLine("3. Ändra kund");
                    Console.WriteLine("4. Ta bort kund");
                    Console.WriteLine("5. Tillbaka till huvudmenyn");

                    Console.WriteLine("Välj alternativ: ");

                    if (!int.TryParse(Console.ReadLine(), out CustomerChoice))
                    {
                        Console.WriteLine("Ange ett giltigt nummer!");
                        continue;
                    }

                    if (CustomerChoice == 1)
                    {
                        customerRepository.GetAllCustomers(connection);
                    }
                    else if (CustomerChoice == 2)
                    {
                        Console.WriteLine("Ange kundens namn:");
                        string Name = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange kundens telefonnummer:");
                        string PhoneNumber = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange kundens adress:");
                        string Address = Console.ReadLine() ?? "";

                        Customer customer = new Customer(
                            0,
                            Name,
                            PhoneNumber,
                            Address);

                        customerRepository.AddCustomer(connection, customer);
                        Console.WriteLine("Kunden har lagts till!");
                    }
                    else if (CustomerChoice == 3)
                    {
                        Console.WriteLine("Vilken kund vill du ändra? Ange ID:");
                        int changeId = Convert.ToInt32(Console.ReadLine());

                        Console.WriteLine("Ange nytt namn:");
                        string changeName = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange nytt telefonnummer:");
                        string changePhone = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange ny adress:");
                        string changeAddress = Console.ReadLine() ?? "";

                        Customer customer = new Customer(
                            changeId,
                            changeName,
                            changePhone,
                            changeAddress);

                        customerRepository.UpdateCustomer(connection, customer);
                    }
                    else if (CustomerChoice == 4)
                    {
                        Console.WriteLine("Vilken kund vill du ta bort? Ange ID:");
                        int removeCustomerId = Convert.ToInt32(Console.ReadLine());

                        customerRepository.DeleteCustomer(
                            connection,
                            removeCustomerId);
                    }
                    else if (CustomerChoice == 5)
                    {
                        Console.WriteLine("Tillbaka till huvudmenyn!");
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt alternativ!");
                    }
                }
            }
            else if (Choice == 3)
            {
                // Hyr film
                Console.WriteLine("------- Hyr film ------");

                Console.Write("Ange kundens ID: ");
                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    Console.WriteLine("Ogiltigt kund-ID.");
                    continue;
                }

                Console.Write("Ange filmexemplarets ID: ");
                if (!int.TryParse(Console.ReadLine(), out int physicalCopyId))
                {
                    Console.WriteLine("Ogiltigt filmexemplar-ID.");
                    continue;
                }

                Console.Write("Har kundens ID kontrollerats? (j/n): ");
                bool idChecked =
                    Console.ReadLine()?.Trim().ToLower() == "j";

                Console.Write("Ange återlämningsdatum (åååå-mm-dd): ");

                if (!DateTime.TryParseExact(
                    Console.ReadLine(),
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime returnDate))
                {
                    Console.WriteLine("Ogiltigt datum.");
                    continue;
                }

                if (returnDate.Date < DateTime.Today)
                {
                    Console.WriteLine(
                        "Återlämningsdatum får inte vara tidigare än idag.");
                    continue;
                }

                Console.Write("Bekräfta uthyrningen? (j/n): ");

                if (Console.ReadLine()?.Trim().ToLower() != "j")
                {
                    Console.WriteLine("Uthyrningen avbröts.");
                    continue;
                }

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
            else if (Choice == 4)
            {
                // Återlämning
                Console.WriteLine("------- Återlämning ------");
                bool continueReturning = true;   //hantering för flera återlämningar
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
                        ReturnService.ReturnMovie(connection,physicalCopyId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Kunde inte återlämna Film: {ex.Message}");
                    }
            }
            Console.Write ("Vill du återlämna yttligare en film? (j/n); ");
            string? answer = Console.ReadLine();
            if(answer?.ToLower()!="j")
                {
                    continueReturning =false;
                }
                }
            }
            else if (Choice == 5)
            {
                // Försening
            }
            else
            {
                Console.WriteLine("Avsluta!");
                break;
            }
        }
    }
}
