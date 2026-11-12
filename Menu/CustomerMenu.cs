using MySqlConnector;

public class CustomerMenu
{
    public static void ShowCustomerMenu(MySqlConnection connection)
    {
           // Kundhantering
                CustomerRepository customerRepository = new CustomerRepository();
                int CustomerChoice = 0;

                while (CustomerChoice != 5)
                {
                    Console.WriteLine("---------Kunder--------");
                    Console.WriteLine("Välj nedan?");
                    Console.WriteLine("1. Visa alla kunder");
                    Console.WriteLine("2. Lägg till kund");
                    Console.WriteLine("3. Ändra kund");
                    Console.WriteLine("4. Ta bort kund");
                    Console.WriteLine("5. Tillbaka till huvudmenyn");

                    Console.WriteLine("Välj alternativ: ");

                    if (!int.TryParse(Console.ReadLine(), out CustomerChoice))
                    {
                        Console.WriteLine("Ange ett giltigt nummer!");
                        continue;
                    }

                    if (CustomerChoice == 1)
                    {
                        customerRepository.GetAllCustomers(connection);
                    }
                    else if (CustomerChoice == 2)
                    {
                        Console.WriteLine("Ange kundens namn:");
                        string Name = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange kundens telefonnummer:");
                        string PhoneNumber = Console.ReadLine() ?? "";

                        Console.WriteLine("Ange kundens adress:");
                        string Address = Console.ReadLine() ?? "";

                        Customer customer = new Customer(
                            0,
                            Name,
                            PhoneNumber,
                            Address);

                        customerRepository.AddCustomer(connection, customer);
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

                        Customer customer = new Customer(
                            changeId,
                            changeName,
                            changePhone,
                            changeAddress);

                        customerRepository.UpdateCustomer(connection, customer);
                    }
                    else if (CustomerChoice == 4)
                    {
                        Console.WriteLine("Vilken kund vill du ta bort? Ange ID:");
                        int removeCustomerId = Convert.ToInt32(Console.ReadLine());

                        customerRepository.DeleteCustomer(
                            connection,
                            removeCustomerId);
                    }
                    else if (CustomerChoice == 5)
                    {
                        Console.WriteLine("Tillbaka till huvudmenyn!");
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt alternativ!");
                    }
                }

    }

}