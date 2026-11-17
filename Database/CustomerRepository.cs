
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

    public bool AddCustomer(MySqlConnection connection, Customer customer)
    {
        using MySqlTransaction transaction =connection.BeginTransaction();
        

        try
        {
        string sql = """
            INSERT INTO Customer (Name, PhoneNumber, Address)
            VALUES (@Name, @PhoneNumber, @Address)
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
        command.Parameters.AddWithValue("@Address", customer.Address);


        int rows = command.ExecuteNonQuery();
        transaction.Commit();
        return rows > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    public List<Customer> GetAllCustomers(MySqlConnection connection)
    {
        List<Customer> customers = new List<Customer>();

        string sql = """
            SELECT Id, Name, PhoneNumber, Address
            FROM Customer
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection);
        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Customer customer = new Customer(
                reader.GetInt32("Id"),
                reader.GetString("Name"),
                reader.GetString("PhoneNumber"),
                reader.GetString("Address")
            );
            customers.Add(customer);
        }
        return customers;
    }
    public bool UpdateCustomer(MySqlConnection connection, Customer customer)
    {
        using MySqlTransaction transaction = connection.BeginTransaction();

        try
        {
        string sql = """
            UPDATE Customer
            SET Name = @Name,
            PhoneNumber = @PhoneNumber,
            Address = @Address
            WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@Id", customer.Id);
        command.Parameters.AddWithValue("@Name", customer.Name);
        command.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
        command.Parameters.AddWithValue("@Address", customer.Address);

        int rows = command.ExecuteNonQuery();
        transaction.Commit();
        return rows > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        
        }
    }
    public bool DeleteCustomer(MySqlConnection connection, int id)
    {
        using MySqlTransaction transaction = connection.BeginTransaction();
        try
        {
        string sql = """
            DELETE FROM Customer WHERE Id = @Id
            """;

        using MySqlCommand command = new MySqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@Id", id);

        int rows = command.ExecuteNonQuery();
        transaction.Commit();
        return rows > 0;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

