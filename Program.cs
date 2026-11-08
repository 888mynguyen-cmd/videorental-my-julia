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
        
    }
}


