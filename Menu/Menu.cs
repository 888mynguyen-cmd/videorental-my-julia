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
            Console.WriteLine("2. ");
            Console.WriteLine("3. ");
            Console.WriteLine("4. ");
            Console.WriteLine("5. ");
            Console.WriteLine("6. ");
            Console.WriteLine("7. ");

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
                    List<VHS> movies = MovieRepository.ShowMovie(connection);

                    // Går igenom varje film i listan, en i taget
                    foreach(VHS movie in movies)
                    {
                        Console.WriteLine($"Titel: ('{movie.Title}', Beskrivning: '{movie.Description}')");
                        
                    }
                }

                else if (FilmChoice == 2)
                {
                    Console.WriteLine("Lägg till id på film ");
                    int Id = Convert.ToInt32(Console.ReadLine());

                    Console.WriteLine("Lägg till en titel på film ");
                    string Title = Console.ReadLine();

                    Console.WriteLine("Lägg till en beskrivning på film ");
                    string Description = Console.ReadLine();

                    Console.WriteLine("Finns filmen på lager?");
                    bool InCatalog = bool.Parse(Console.ReadLine());

                    MovieRepository.AddMovie(connection, Id, Title, Description, InCatalog);
                }
            }

            else if (Choice == 2)
            {
                        
            }

            else if (Choice == 3)
            {
                        
            }

            else if (Choice == 4)
            {
                        
            }

            else if (Choice == 5)
            {
                        
            }

            else if (Choice == 6)
            {
                        
            }

            else if (Choice == 7)
            {
                        
            }
            else
            {
                Console.WriteLine("Avsluta!");
                break;
            }
        }
    }
}