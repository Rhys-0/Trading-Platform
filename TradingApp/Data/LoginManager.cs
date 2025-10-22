using Npgsql;
using Dapper;
using TradingApp.Data.Interfaces;
using TradingApp.Models;

namespace TradingApp.Data {
    internal class LoginManager : ILoginManager {
        private readonly DatabaseConnection _connection;
        public LoginManager(DatabaseConnection connection) {
            _connection = connection;
        }

        /// <summary>
        /// Return the a user object given the email and hashed password of a user, used to validate user logins.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="hashedPassword">The password of the user</param>
        /// <returns>A User object containing all the information from the user table in the database,
        /// if no user with the given email and password is found, the function returns null.</returns>
        public async Task<User?> RetrieveUser(string email, string hashedPassword) {
            using var connection = await _connection.CreateConnectionAsync();
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT user_id AS id, " +
                "username, " +
                "email, " +
                "first_name AS firstName, " +
                "last_name AS lastName, " +
                "starting_cash_balance AS startingCashBalance, " +
                "current_cash_balance AS currentCashBalance " +
                "FROM users " +
                "WHERE email = @Email and password_hash = @HashedPassword",
                new { Email = email, HashedPassword = hashedPassword });

            if (user == null)
                return null;

            await LoadPortfolioDataAsync(connection, user);

            return user;
        }

        /// <summary>
        /// Return the a user object given the username and hashed password of a user, used to validate user logins.
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="hashedPassword">The password of the user</param>
        /// <returns>A User object containing all the information from the user table in the database,
        /// if no user with the given username and password is found, the function returns null.</returns>
        public async Task<User?> RetrieveUserByUsername(string username, string hashedPassword) {
            using var connection = await _connection.CreateConnectionAsync();
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT user_id AS id, " +
                "username, " +
                "email, " +
                "first_name AS firstName, " +
                "last_name AS lastName, " +
                "starting_cash_balance AS startingCashBalance, " +
                "current_cash_balance AS currentCashBalance " +
                "FROM users " +
                "WHERE username = @Username and password_hash = @HashedPassword",
                new { Username = username, HashedPassword = hashedPassword });

            if (user == null)
                return null;

            await LoadPortfolioDataAsync(connection, user);

            return user;
        }

        /// <summary>
        /// Get a database connection for external use.
        /// </summary>
        /// <returns>A database connection</returns>
        public async Task<NpgsqlConnection> GetConnectionAsync() {
            return (NpgsqlConnection)await _connection.CreateConnectionAsync();
        }

        /// <summary>
        /// Add a user to the database, typically on the registration page.
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="passwordHash">The password of the user</param>
        /// <param name="firstName">The first name of the user</param>
        /// <param name="lastName">The surname of the user</param>
        /// <returns>True if the user was added successfully, false otherwise</returns>
        public async Task<bool> AddUser(string username, string email, string passwordHash, string firstName, string lastName) {
            decimal startingCash = 10_000.00m;

            using var connection = await _connection.CreateConnectionAsync();
            int rowsAffected = 0;
            try {
                rowsAffected = await connection.ExecuteAsync(
                "INSERT into users" +
                "(username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance) values" +
                "(@Username, @Email, @PasswordHash, @FirstName, @LastName, @StartingCashBalance, @CurrentCashBalance)",
                new {
                    Username = username,
                    Email = email,
                    PasswordHash = passwordHash,
                    FirstName = firstName,
                    LastName = lastName,
                    StartingCashBalance = startingCash,
                    CurrentCashBalance = startingCash
                }
                );
            } catch (PostgresException ex) when (ex.SqlState == "23505") {
                // email already exists
                return false;
            }

            if (rowsAffected != 1) return false;

            // Create the user's portfolio
            try {
                var userId = await connection.QuerySingleAsync<long>(
                    "SELECT user_id FROM users WHERE email = @Email",
                    new { Email = email });

                // First, ensure the portfolio table exists
                Console.WriteLine("🔍 DB DEBUG: Creating portfolio table if it doesn't exist...");
                await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS portfolio (
                        user_id BIGINT PRIMARY KEY REFERENCES users(user_id),
                        value DECIMAL(18,2) NOT NULL DEFAULT 0.00,
                        net_profit DECIMAL(18,2) NOT NULL DEFAULT 0.00,
                        percentage_return DECIMAL(5,2) NOT NULL DEFAULT 0.00,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    )");
                Console.WriteLine("🔍 DB DEBUG: Portfolio table creation completed");

                rowsAffected = await connection.ExecuteAsync(
                    "INSERT INTO portfolio " +
                    "(user_id, value, net_profit, percentage_return) " +
                    "VALUES " +
                    "(@UserId, @Value, @NetProfit, @PercentageReturn)",
                    new {
                        UserId = userId,
                        Value = 0.00m,
                        NetProfit = 0.00m,
                        PercentageReturn = 0.00m
                    }
                );

                Console.WriteLine($"🔍 DB DEBUG: Portfolio created successfully for user {userId}");
                return (rowsAffected == 1);
            } catch (PostgresException ex) when (ex.SqlState == "42P01") {
                // Portfolio table creation failed
                Console.WriteLine($"🔍 DB DEBUG: Failed to create portfolio table: {ex.Message}");
                return false;
            } catch (Exception ex) {
                // Portfolio creation failed
                Console.WriteLine($"🔍 DB DEBUG: Portfolio creation failed: {ex.Message}");
                return false;
            }
        }

        private async Task LoadPortfolioDataAsync(System.Data.IDbConnection connection, User user)
        {
                var portfolio = await connection.QueryFirstOrDefaultAsync<Portfolio>(
                "SELECT portfolio_id AS PortfolioId, " +
                "value, " +
                "net_profit AS NetProfit, " +
                "percentage_return AS PercentageReturn " +
                "FROM portfolio " +
                "WHERE user_id = @UserId",
                new { UserId = user.Id });

            if (portfolio == null)
            {
                user.Portfolio = new Portfolio(0, 0, 0, 0)
                {
                    Positions = new Dictionary<string, Position>()
                };
                return;
            }

            user.Portfolio = portfolio;

            var positions = await connection.QueryAsync<Position>(
                "SELECT position_id AS PositionId, " +
                "stock_symbol AS StockSymbol, " +
                "total_quantity AS TotalQuantity " +
                "FROM position " +
                "WHERE portfolio_id = @PortfolioId",
                new { PortfolioId = portfolio.PortfolioId });

            portfolio.Positions = positions.ToDictionary(p => p.StockSymbol, p => p);

            foreach (var position in portfolio.Positions.Values)
            {
                var lots = await connection.QueryAsync<PurchaseLot>(
                    "SELECT purchase_lot_id AS PurchaseLotId, " +
                    "position_id AS PositionId, " +
                    "quantity, " +
                    "purchase_price AS PurchasePrice, " +
                    "purchase_date AS PurchaseDate " +
                    "FROM purchase_lot " +
                    "WHERE position_id = @PositionId",
                    new { PositionId = position.PositionId });

                position.PurchaseLots = lots.ToList();
            }
        }

    }
}
