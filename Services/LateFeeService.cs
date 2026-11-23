
using MySqlConnector;

public class LateFeeService
{
    public static List<LateRental> GetLateRentals(
        MySqlConnection connection)
    {
        List<LateRental> lateRentals = LateFeeRepository.GetLateRentals(connection);

        foreach (LateRental rental in lateRentals)
        {
            int lateDays = Math.Max(
                0,
                (DateTime.Today - rental.ReturnDate.Date).Days);

            rental.LateDays = lateDays;
            rental.LateFee = lateDays * rental.DailyPrice;
        }

        return lateRentals;

    }
}
