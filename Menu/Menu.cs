using MySqlConnector;

public class Menu // Här har vi själva menyn
{
    public static void ShowMenu(MySqlConnection connection)
    {
        while(true)
        {
            Console.WriteLine("---------Videoteket--------");
            Console.WriteLine("Vad vill du göra idag?");

            Console.WriteLine("1. Filmer");
            Console.WriteLine("2. Kunder");
            Console.WriteLine("3. Hyr Film ");
            Console.WriteLine("4. Återlämning");
            Console.WriteLine("5. Försening");

            Console.WriteLine("Välj alternativ: ");
            int Choice = Convert.ToInt32(Console.ReadLine());

            if (Choice == 1)
            {
                Console.WriteLine("---------Filmer--------");
                Console.WriteLine("Välj nedan?");

                Console.WriteLine("1. Visa alla Filmer");
                Console.WriteLine("2. Lägg till film");
                Console.WriteLine("3. Ändra film");
                Console.WriteLine("4. Ta bort");
                Console.WriteLine("5. ");

                Console.WriteLine("Välj alternativ: ");
                int FilmChoice = Convert.ToInt32(Console.ReadLine());

                if (FilmChoice == 1)
                {
                    // Anropar ShowMovie metoden i MovieRepository och sparar listan med filmer
                    List<Movie> movies = MovieRepository.ShowMovie(connection);

                    // Går igenom varje film i listan, en i taget
                    foreach(Movie movie in movies)
                    {
                        Console.WriteLine($"Titel: ('{movie.Title}', Beskrivning: '{movie.Description}')");
                        
                    }
                }

                else if (FilmChoice == 2)
                {
                    Console.WriteLine("Lägg till id på film ");
                    int Id = Convert.ToInt32(Console.ReadLine() ?? "");

                    Console.WriteLine("Lägg till en titel på film ");
                    string Title = Console.ReadLine() ?? "";

                    Console.WriteLine("Lägg till en beskrivning på film ");
                    string Description = Console.ReadLine() ?? "";

                    Console.WriteLine("Vilket id på priset till filmen?"); // 1 Är Pris för Ny Film, 2 är Pris för gammal film
                    int RentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");

                    MovieRepository.AddMovie(connection, Id, Title, Description, RentalPriceId);
                }

                else if (FilmChoice == 3)
                {
                    // Ändra en film
                    Console.WriteLine("Vilken film vill du ändra?");
                    int changeId = Convert.ToInt32(Console.ReadLine() ?? "");

                    Console.WriteLine("Vad vill du ändra filmens titel till?");
                    string changeTitle = Console.ReadLine() ?? "";
                    
                    Console.WriteLine("Vad vill du ändra filmens beskrivning till?");
                    string changeDescription = Console.ReadLine() ?? "";

                    Console.WriteLine("Vad vill du ändra filmens Pris till?");
                    int changeRentalPriceId = Convert.ToInt32(Console.ReadLine() ?? "");


                    MovieRepository.ChangeMovie(connection,changeId, changeTitle, changeDescription, changeRentalPriceId);
                }

                else if (FilmChoice == 4)
                {
                    // Ta bort en film
                    Console.WriteLine("Vilken film vill du ta bort?");
                    int removeMovieId = Convert.ToInt32(Console.ReadLine() ?? "");

                    MovieRepository.RemoveMovie(connection, removeMovieId);
                    Console.WriteLine("Filmen har tagits bort!");

                }
            }
                else if (Choice == 2)
                {
            
                Console.WriteLine("---------Kunder--------");
                Console.WriteLine("Välj nedan?");
                Console.WriteLine("1. Visa alla kunder");
                Console.WriteLine("2. Lägg till kund");
                Console.WriteLine("3. Ändra kund");
                Console.WriteLine("4. Ta bort kund");
                Console.WriteLine("Välj alternativ: ");
                int CustomerChoice = Convert.ToInt32(Console.ReadLine());
                CustomerRepository customerRepository = new CustomerRepository();
                if (CustomerChoice == 1)
                {
                    customerRepository.GetAllCustomers();
                }
                else if (CustomerChoice == 2)
                {
                    Console.WriteLine("Ange kundens namn:");
                    string Name = Console.ReadLine() ?? "";
                    Console.WriteLine("Ange kundens telefonnummer:");
                    string PhoneNumber = Console.ReadLine() ?? "";
                    Console.WriteLine("Ange kundens adress:");
                    string Address = Console.ReadLine() ?? "";
                    Customer customer = new Customer(0, Name, PhoneNumber, Address);
                customerRepository.AddCustomer(customer);
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
                    Customer customer = new Customer(changeId, changeName, changePhone, changeAddress);
                customerRepository.UpdateCustomer(customer);
                }
                else if (CustomerChoice == 4)
                {
                    Console.WriteLine("Vilken kund vill du ta bort? Ange ID:");
                    int removeCustomerId = Convert.ToInt32(Console.ReadLine());
                    customerRepository.DeleteCustomer(removeCustomerId);
                }
                else
                {
                    Console.WriteLine("Ogiltigt alternativ!");
                    continue;
                }
            }

            else if (Choice == 2)
            {
             // Kund           

            }

            else if (Choice == 3)
            {
                // Hyr film        
            }

            else if (Choice == 4)
            {
                // Återlämning
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