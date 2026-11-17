using MySqlConnector;

public class Program
{
    public static void Main()
    {
        DotNetEnv.Env.Load(); // Läser in lösenordet från .env-filen så att det inte behöver stå direkt i koden.
        // Skapa en connection string
        string? password = Environment.GetEnvironmentVariable("DB_PASSWORD");

    if (string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("FEL: DB_PASSWORD hittades inte i .env-filen!");
        return;
    }
        string connectionString = 
            $"""
            Server={Environment.GetEnvironmentVariable("DB_SERVER")};
            Port={Environment.GetEnvironmentVariable("DB_PORT")};
            Database={Environment.GetEnvironmentVariable("DB_DATABASE")};
            User={Environment.GetEnvironmentVariable("DB_USER")};
            Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};
            """;

            using MySqlConnection connection = new MySqlConnection(connectionString);
            
            connection.Open();

            Menu.ShowMenu(connection);
    }


}