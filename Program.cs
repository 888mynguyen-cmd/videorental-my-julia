using MySqlConnector;

public class Program
{
    public static void Main()
    {
        DotNetEnv.Env.Load();
        // SKapa en connection string
        string connectionString = 
            $"""
            Server=comet-direct.usbx.me;
            Port=17815;
            Database=yh_julia_Videoteket;
            User=yh_julia;
            Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}
            """;

            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

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

                    ShowMovie(connection);

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

    public static void ShowMovie(MySqlConnection connection)
    {

        using MySqlCommand command = new MySqlCommand("SELECT Name, Description FROM VHS;", connection); // Här skapar vi command för SELECT

        using MySqlDataReader reader = command.ExecuteReader();

        while(reader.Read())
        {
            String name = reader.GetString("Name");
            String description = reader.GetString("Description");

            Console.WriteLine($"Namn: ('{name}', Beskrivning: '{description}')");
        }
    }
}


