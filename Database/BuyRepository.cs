    using MySqlConnector;

    public class BuyRepository
    // Hit behöver vi flytta all SQL queries från service mappen i filen BuyService och separera Service logiken från Databas (SQL koden)
    {
        public static void AddProductToOrder(
            MySqlConnection connection,
            int cOrderId,
            int productId,
            int quantity)
        {

            using var transaction = connection.BeginTransaction();

            try
            {
                // Kontrollera att ordern finns och inte är betald
                const string orderSql = """
                    SELECT Paid
                    FROM COrder
                    WHERE Id = @cOrderId
                    FOR UPDATE
                    """;

                using (var command = new MySqlCommand(
                    orderSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@cOrderId", cOrderId);

                    object? result = command.ExecuteScalar();

                    if (result == null)
                        throw new Exception("Beställningen finns inte.");

                    if (Convert.ToBoolean(result))
                        throw new Exception("Beställningen är redan betald.");
                }

                // Hämta produktens pris och lagersaldo
                decimal unitPrice;
                int stock;

                const string productSql = """
                    SELECT Price, Stock
                    FROM Product
                    WHERE Id = @productId
                    FOR UPDATE
                    """;

                using (var command = new MySqlCommand(
                    productSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@productId", productId);

                    using var reader = command.ExecuteReader();

                    if (!reader.Read())
                        throw new Exception("Produkten finns inte.");

                    unitPrice = reader.GetDecimal("Price");
                    stock = reader.GetInt32("Stock");
                }

                if (stock < quantity)
                    throw new Exception("Det finns inte tillräckligt i lager.");

                // Registrera köpet
                const string buySql = """
                    INSERT INTO Buy
                    (Quantity, PriceAtTransaction, COrderId, ProductId)
                    VALUES
                    (@quantity, @price, @cOrderId, @productId)
                    """;

                using (var command = new MySqlCommand(
                    buySql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@price", unitPrice);
                    command.Parameters.AddWithValue("@cOrderId", cOrderId);
                    command.Parameters.AddWithValue("@productId", productId);

                    command.ExecuteNonQuery();
                }

                // Minska lagersaldot
                const string stockSql = """
                    UPDATE Product
                    SET Stock = Stock - @quantity
                    WHERE Id = @productId
                    """;

                using (var command = new MySqlCommand(
                    stockSql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@productId", productId);

                    command.ExecuteNonQuery();
                }

                // Uppdatera orderns totalsumma
                const string totalSql = """
                    UPDATE COrder
                    SET Total = Total + @total
                    WHERE Id = @cOrderId
                    """;

                using (var command = new MySqlCommand(
                    totalSql, connection, transaction))
                {
                    command.Parameters.AddWithValue(
                        "@total", unitPrice * quantity);
                    command.Parameters.AddWithValue("@cOrderId", cOrderId);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }