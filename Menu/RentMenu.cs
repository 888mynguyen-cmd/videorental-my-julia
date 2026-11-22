
using MySqlConnector;
using System.Globalization;

public class RentMenu
{
    public static void ShowRentMenu(MySqlConnection connection)
    {
        Console.WriteLine("------- Hyr film -------");

        Console.Write("Ange kundens ID: ");

        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Ogiltigt kund-ID.");
            return;
        }

        if (!RentalService.CustomerExists(connection, customerId))
        {
            Console.WriteLine("Kunden finns inte.");
            return;
        }

        Console.Write("Har kundens ID kontrollerats? (j/n): ");
        bool idChecked =
            Console.ReadLine()?.Trim().ToLower() == "j";

        int? cOrderId = null;
        bool continueRenting = true;

        while (continueRenting)
        {
            Console.WriteLine();
            Console.WriteLine("------- Hyr film -------");

            Console.Write("Ange filmexemplarets ID: ");

            if (!int.TryParse(
                Console.ReadLine(), out int physicalCopyId))
            {
                Console.WriteLine("Ogiltigt filmexemplar-ID.");
            }
            else
            {
                Console.Write(
                    "Ange återlämningsdatum (yyyy-MM-dd): ");

                if (!DateTime.TryParseExact(
                    Console.ReadLine(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime returnDate))
                {
                    Console.WriteLine("Ogiltigt datum.");
                }
                else if (returnDate.Date < DateTime.Today)
                {
                    Console.WriteLine(
                        "Återlämningsdatum får inte vara tidigare än idag.");
                }
                else
                {
                    Console.Write("Bekräfta uthyrningen? (j/n): ");

                    if (Console.ReadLine()?.Trim().ToLower() != "j")
                    {
                        Console.WriteLine("Uthyrningen avbröts.");
                    }
                    else
                    {
                        try
                        {
                            int orderId = RentalService.RentMovie(
                                connection,
                                customerId,
                                physicalCopyId,
                                idChecked,
                                returnDate,
                                cOrderId);

                            cOrderId = orderId;

                            Console.WriteLine(
                                $"Uthyrningen registrerades! Order-ID: {orderId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Kunde inte hyra ut filmen: {ex.Message}");
                        }
                    }
                }
            }

            Console.Write(
                "Vill du hyra ut ytterligare en film? (j/n): ");

            continueRenting =
                Console.ReadLine()?.Trim().ToLower() == "j";
        }

        // Köp produkter på samma order
        if (cOrderId.HasValue)
        {
            Console.Write("Vill kunden köpa produkter? (j/n): ");

            bool continueBuying =
                Console.ReadLine()?.Trim().ToLower() == "j";

            while (continueBuying)
            {
                Console.Write("Ange produktens ID: ");

                if (!int.TryParse(
                    Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Ogiltigt produkt-ID.");
                }
                else
                {
                    Console.Write("Ange antal: ");

                    if (!int.TryParse(
                        Console.ReadLine(), out int quantity) ||
                        quantity <= 0)
                    {
                        Console.WriteLine("Ogiltigt antal.");
                    }
                    else
                    {
                        try
                        {
                            BuyService.AddProductToOrder(
                                connection,
                                cOrderId.Value,
                                productId,
                                quantity);

                            Console.WriteLine(
                                "Produkten har lagts till i beställningen!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Kunde inte köpa produkten: {ex.Message}");
                        }
                    }
                }

                Console.Write(
                    "Vill kunden köpa ytterligare en produkt? (j/n): ");

                continueBuying =
                    Console.ReadLine()?.Trim().ToLower() == "j";
            }

            Console.Write("Vill kunden ha ett kvitto? (j/n): ");

            if (Console.ReadLine()?.Trim().ToLower() == "j")
            {
                try
                {
                    ReceiptService receipt = new ReceiptService();

                    receipt.LoadOrderFromDatabase(
                        connection, cOrderId.Value);

                    receipt.PrintReceipt();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Kunde inte skriva ut kvittot: {ex.Message}");
                }
            }
        }
    }
}
