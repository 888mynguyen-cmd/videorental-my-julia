using MySqlConnector;

public class Menu // Här har vi själva menyn
{
    public static void ShowMenu(MySqlConnection connection)
    {
        while(true)
        {
            Console.WriteLine("---------Videoteket--------");
            Console.WriteLine("Vad vill du göra idag?");

            Console.WriteLine("1. Visa alla filmer");
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
                // Anropar ShowMovie metoden i MovieRepository och sparar listan med filmer
                List<VHS> movies = MovieRepository.ShowMovie(connection);

                // Går igenom varje film i listan, en i taget
                foreach(VHS movie in movies)
                {
                    Console.WriteLine($"Titel: ('{movie.Title}', Beskrivning: '{movie.Description}')");
                    
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