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
                List<Customer> customers = customerRepository.GetAllCustomers(connection);
                foreach(Customer customer in customers)
                {
                    Console.WriteLine($"{customer.Id}|{customer.Name}|{customer.PhoneNumber}|{customer.Address}");
                }
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

                bool added = customerRepository.AddCustomer(connection, customer);
                if (added)
                {
                Console.WriteLine("Kunden har lagts till!");
                }
                else
                {
                    Console.WriteLine("Kunden kunde inte läggas till!");
                }
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

                bool updated = customerRepository.UpdateCustomer(connection, customer);
                if (updated)
                {
                    Console.WriteLine("Kunden har uppdaterats!");
                }
                else
                {
                    Console.WriteLine("Kunden hittades inte!");
                }
            }
            else if (CustomerChoice == 4)
            {
                Console.WriteLine("Vilken kund vill du ta bort? Ange ID:");
                int removeCustomerId = Convert.ToInt32(Console.ReadLine());

                bool deleted = customerRepository.DeleteCustomer(connection, removeCustomerId);
                if (deleted)
                {
                    Console.WriteLine("Kunden har tagits bort!");
                }
                else
                {
                    Console.WriteLine("Kunden hittades inte!");
                }

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