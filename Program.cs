using MySqlConnector;

public class Program
{
    public static void Main()
    {
        DotNetEnv.Env.Load(); // Läser in lösenordet från .env-filen så att det inte behöver stå direkt i koden.
        // Skapa en connection string
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

            Menu.ShowMenu(connection);
        
    }


}