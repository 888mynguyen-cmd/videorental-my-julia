using MySqlConnector;
public class ReturnService
{
    private const decimal LateFeePerDays = 10;

    public static decimal ReturnMovie(MySqlConnection connection, int physicalCopyId)
    {
        using var transaction = connection.BeginTransaction();

        try
        {
            const string sql = """
                SELECT RentItem.RentId, RentItem.ReturnDate
                FROM RentItem
                JOIN PhysicalCopy
                ON RentItem.PhysicalCopyId = PhysicalCopy.Id
                JOIN FilmStatus
                ON PhysicalCopy.FilmStatusId = FilmStatus.Id
                WHERE PhysicalCopy.Id = @physicalCopyId
                AND FilmStatus.Name = 'Rented'
                AND RentItem.ActualReturnDate IS NULL
                FOR UPDATE
                """;

            int rentId;
            DateTime returnDate;
            DateTime actualReturnDate = DateTime.Today;

            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                using var reader = command.ExecuteReader();
                if (!reader.Read())
                {
                    throw new Exception("Filmen är inte uthyrd eller finns inte.");
                }

                rentId = reader.GetInt32(0);
                returnDate = reader.GetDateTime(1);
            }

            DateTime actualRentalDate = DateTime.Today;
            int lateDays = Math.Max(0, (actualRentalDate.Date - returnDate.Date).Days);
            decimal lateFee = lateDays * LateFeePerDays;

            const string updateRentItemSql = """
                UPDATE RentItem
                SET ActualReturnDate = @actualReturnDate,
                    LateFee = @lateFee
                WHERE RentId = @rentId
                  AND PhysicalCopyId = @physicalCopyId
                  AND ActualReturnDate IS NULL
                """;

            using (var command = new MySqlCommand(updateRentItemSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@actualReturnDate", actualReturnDate);
                command.Parameters.AddWithValue("@lateFee", lateFee);
                command.Parameters.AddWithValue("@rentId", rentId);
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);

                command.ExecuteNonQuery();
            }

            const string statusSql = """
                SELECT Id
                FROM FilmStatus
                WHERE Name = 'Available'
                """;

            int availableStatusId;

            using (var command = new MySqlCommand(statusSql, connection, transaction))
            {
                object? result = command.ExecuteScalar();
                if (result == null)
                {
                    throw new Exception("Statusen Available saknas");
                }

                availableStatusId = Convert.ToInt32(result);
            }

            const string updateCopySql = """
                UPDATE PhysicalCopy
                SET FilmStatusId = @statusId
                WHERE Id = @physicalCopyId
                """;

            using (var command = new MySqlCommand(updateCopySql, connection, transaction))
            {
                command.Parameters.AddWithValue("@statusId", availableStatusId);
                command.Parameters.AddWithValue("@physicalCopyId", physicalCopyId);
                command.ExecuteNonQuery();
            }

            transaction.Commit();
            return lateFee;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}