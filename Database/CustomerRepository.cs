
using System;
using System.Data.Common;
using MySqlConnector;

public class CustomerRepository
{

    private static DbConnection GetConnection()
    {
        string? connectionString = Environment.GetEnvironmentVariable("VIDEORENTAL_CONNECTION_STRING");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Set the VIDEORENTAL_CONNECTION_STRING environment variable.");
        }

        return new MySqlConnection(connectionString);
    }

    public void AddCustomer(MySqlConnection connection, Customer customer)
    {
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

    public void GetAllCustomers(MySqlConnection connection)
    {
       

        string sql = """
            SELECT * FROM Customer
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(
                $"{reader["Id"]}|"+ $"{reader["Name"]}|"+$"{reader["PhoneNumber"]}|"+$"{reader["Address"]}"
            );
        }
    }
    public void UpdateCustomer(MySqlConnection connection,Customer customer)
    {
       
        string sql = """
            UPDATE Customer
            SET Name = @Name,
            PhoneNumber = @PhoneNumber,
            Address = @Address
            WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", customer.Id);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
        command.Parameters.AddWithValue("@Address", customer.Address);

        int rows = command.ExecuteNonQuery();
        Console.WriteLine(rows > 0 ? "Uppdatera kunden!" : "Kunden hittades inte!");
    }

    public void DeleteCustomer(MySqlConnection connection, int id)
    {
        string sql = """
            DELETE FROM Customer WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        int rows = command.ExecuteNonQuery();
        Console.WriteLine(rows > 0 ? "Kunden borttagen!" : "Kunden hittades inte!");
    }
}