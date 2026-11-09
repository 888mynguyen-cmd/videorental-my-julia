
using System;
using System.Data.Common;
using MySqlConnector;

public class DatabaseConnection
{
    private readonly string connectionString =
        $"""
        Server=comet-direct.usbx.me;
        Port=17815;
        Database=yh_julia_Videoteket;
        User=yh_julia;
        Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}
        """;

    public DbConnection GetConnection()
    {
        return new MySqlConnection(connectionString);
    }

    public void AddCustomer(Customer customer)
    {
        using MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();

        string sql = """
            INSERT INTO Customer (Name, PhoneNumber, Address)
            VALUES (@Name, @PhoneNumber, @Address)
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
        command.Parameters.AddWithValue("@Address", customer.Address);

        command.ExecuteNonQuery();
    }

    public void GetAllCustomers()
    {
        using DbConnection conn = GetConnection();
        conn.Open();

        string sql = """
            SELECT * FROM Customer
            """;

        using MySqlCommand command = new MySqlCommand(sql, (MySqlConnection)conn);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader["Id"]}|"+ $"{reader["Name"]}|"+$"{reader["PhoneNumber"]}|"+$"{reader["Address"]}"
            );
        }
    }
    public void UpdateCustomer(Customer customer)
    {
        using DbConnection conn = GetConnection();
        conn.Open();

        string sql = """
            UPDATE Customer
            SET Name = @Name,
            PhoneNumber = @PhoneNumber,
            Address = @Address
            WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, (MySqlConnection)conn);
        command.Parameters.AddWithValue("@Id", customer.Id);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
        command.Parameters.AddWithValue("@Address", customer.Address);

        int rows = command.ExecuteNonQuery();
        Console.WriteLine(rows > 0 ? "Uppdatera kunden!" : "Kunden hittades inte!");
    }

    public void DeleteCustomer(int id)
    {
        using DbConnection conn = GetConnection();
        conn.Open();

        string sql = """
            DELETE FROM Customer WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, (MySqlConnection)conn);
        command.Parameters.AddWithValue("@Id", id);

        int rows = command.ExecuteNonQuery();
        Console.WriteLine(rows > 0 ? "Kunden borttagen!" : "Kunden hittades inte!");
    }
}