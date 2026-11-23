
using MySqlConnector;

public class ReturnService
{
 
    public static ReturnReceipt ReturnMovie(
        MySqlConnection connection,
        int physicalCopyId,
        bool lateFeePaid = false,
        bool invoice = false)
    {
        using var transaction = connection.BeginTransaction();

        try
        {
            var rental = RentRepository.GetReturnInformation(
                connection,
                transaction,
                physicalCopyId);

            if (rental == null)
                throw new Exception(
                    "Filmen är inte uthyrd eller finns inte.");

            int rentId = rental.RentId;
            DateTime returnDate = rental.ReturnDate;
            string customerName = rental.CustomerName;
            string movieTitle = rental.MovieTitle;
            decimal dailyPrice = rental.DailyPrice;

            DateTime actualReturnDate = DateTime.Today;
            

            int lateDays = Math.Max(
                0,
                (actualReturnDate.Date - returnDate.Date).Days);

            decimal lateFee = lateDays * dailyPrice;

            if (lateFee > 0 && lateFeePaid == invoice)
            {
                throw new Exception(
                    "Välj antingen direktbetalning eller faktura.");
            }

            if (lateFee == 0)
            {
                lateFeePaid = false;
                invoice = false;
            }

            RentRepository.UpdateReturnedRent(
                connection,
                transaction,
                rentId,
                actualReturnDate,
                lateFee,
                lateFeePaid,
                invoice);
                
            int availableStatusId = RentRepository.GetAvailableStatusId(
                connection,
                transaction);
                
            RentRepository.UpdatePhysicalCopyStatus(
                connection,
                transaction,
                physicalCopyId,
                availableStatusId);

            transaction.Commit();

            return new ReturnReceipt
            {
                CustomerName = customerName,
                MovieTitle = movieTitle,
                PhysicalCopyId = physicalCopyId,
                ReturnDate = returnDate,
                ActualReturnDate = actualReturnDate,
                LateFee = lateFee
            };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
    
    public static decimal GetLateFee(
        MySqlConnection connection, int physicalCopyId)
    {
        var rental = RentRepository.GetRentalPrice(connection, physicalCopyId);
        
        if (rental == null)
            throw new Exception(
                "Filmen är inte uthyrd eller finns inte.");
        
        DateTime returnDate = rental.Value.ReturnDate;
        decimal dailyPrice = rental.Value.DailyPrice;

        int lateDays = Math.Max(
            0, (DateTime.Today - returnDate.Date).Days);
        return lateDays * dailyPrice;
    }
}
