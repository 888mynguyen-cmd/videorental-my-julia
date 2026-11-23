using MySqlConnector;

public class RentalService
{
    // Skapa en ny order och hyr den första filmen.
    public static int RentMovie(
        MySqlConnection connection,
        int customerId,
        int physicalCopyId,
        bool idChecked,
        DateTime returnDate)
    {
        return RentMovie(
            connection,
            customerId,
            physicalCopyId,
            idChecked,
            returnDate,
            null,
            null);
    }

    // Lägg till en film i en befintlig order,
    // eller skapa en ny order om cOrderId är null.
    // specialPrice = null betyder automatiskt pris.
    public static int RentMovie(
        MySqlConnection connection,
        int customerId,
        int physicalCopyId,
        bool idChecked,
        DateTime returnDate,
        int? cOrderId,
        decimal? specialPrice = null)
    {
        DateTime rentalDate = DateTime.Today;

        if (returnDate.Date < rentalDate)
            throw new Exception(
                "Återlämningsdatum kan inte vara i det förflutna.");

        if (specialPrice.HasValue &&
            (specialPrice.Value < 0 ||
             specialPrice.Value > 99999999.99m))
        {
            throw new Exception("Ogiltigt specialpris.");
        }

        int rentalDays = Math.Max(
            1,
            (returnDate.Date - rentalDate).Days);

        using var transaction = connection.BeginTransaction();

        try
        {
            // Kontrollera kunden.
            if (!RentRepository.CustomerExists(
                connection, transaction, customerId))
            {
                throw new Exception("Kunden finns inte.");
            }

            // Kontrollera befintlig order.

            if (cOrderId.HasValue)
            {
                var orderInfo = RentRepository.GetOrderInfo(
                    connection, transaction, cOrderId.Value);

                if (!orderInfo.HasValue)
                    throw new Exception("Beställningen finns inte.");

                if (orderInfo.Value.CustomerId != customerId)
                    throw new Exception("Beställningen tillhör en annan kund.");

                if (orderInfo.Value.Paid)
                    throw new Exception("Beställningen är redan betald.");
            }

            // Hämta filmexemplaret och priset.
            var copyInfo = RentRepository.GetRentalCopyInfo(
                connection, transaction, physicalCopyId);

            if (!copyInfo.HasValue)
                throw new Exception("Filmexemplaret eller priset saknas.");

            int statusId = copyInfo.Value.StatusId;
            bool buyOrRent = copyInfo.Value.BuyOrRent;
            decimal dailyPrice = copyInfo.Value.DailyPrice;

            // BuyOrRent = 1 betyder köp.
            if (buyOrRent)
                throw new Exception(
                    "Filmexemplaret är till försäljning och kan inte hyras.");

            var rentalStatuses = RentRepository.GetRentalStatuses(
                connection, transaction);

            if (!rentalStatuses.HasValue)
            {
                throw new Exception(
                    "FilmStatus saknar Available eller Rented.");
            }

            int availableStatusId =
                rentalStatuses.Value.AvailableStatusId;

            int rentedId =
                rentalStatuses.Value.RentedStatusId;

            if (statusId != availableStatusId)
                throw new Exception(
                    "Filmexemplaret är inte tillgängligt.");

            // Använd specialpris om det finns.
            // Annars används automatiskt pris.
            decimal rentalTotal =
                specialPrice ?? (dailyPrice * rentalDays);

            int orderId;

            if (cOrderId.HasValue)
            {
                orderId = cOrderId.Value;
            }
            else
            {
                orderId = RentRepository.CreateOrder(
                    connection,
                    transaction,
                    idChecked,
                    customerId);
            }
            
            // Registrera filmen med överenskommet pris.
            RentRepository.CreateRent(
                connection,
                transaction,
                physicalCopyId,
                orderId,
                rentalTotal,
                returnDate);
            
            // Uppdatera orderns totalbelopp.
            RentRepository.UpdateOrderTotal(
                connection,
                transaction,
                orderId,
                rentalTotal,
                idChecked);
            
            // Ändra filmens status till Rented.
            RentRepository.UpdateCopyStatus(
                connection,
                transaction,
                physicalCopyId,
                rentedId);

            transaction.Commit();

            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
