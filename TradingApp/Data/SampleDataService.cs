using Npgsql;
using Dapper;
using TradingApp.Data.Interfaces;

namespace TradingApp.Data {
    public class SampleDataService {
        private readonly DatabaseConnection _connection;
        private readonly ILogger<SampleDataService> _logger;

        public SampleDataService(DatabaseConnection connection, ILogger<SampleDataService> logger) {
            _connection = connection;
            _logger = logger;
        }

        public async Task SeedSampleUsersAsync() {
            try {
                using var connection = await _connection.CreateConnectionAsync();
                
                // Check if we already have sample data
                var existingUsers = await connection.QuerySingleOrDefaultAsync<int>("SELECT COUNT(*) FROM users");
                if (existingUsers > 1) {
                    _logger.LogInformation("Sample data already exists, skipping seed");
                    return;
                }

                var sampleUsers = new[] {
                    new { Username = "trader_jane", Email = "jane@example.com", FirstName = "Jane", LastName = "Smith", StartingCash = 10000m, CurrentCash = 12500m },
                    new { Username = "stock_guru", Email = "guru@example.com", FirstName = "Mike", LastName = "Johnson", StartingCash = 10000m, CurrentCash = 8750m },
                    new { Username = "crypto_king", Email = "king@example.com", FirstName = "Alex", LastName = "Davis", StartingCash = 10000m, CurrentCash = 15200m },
                    new { Username = "day_trader", Email = "trader@example.com", FirstName = "Sarah", LastName = "Wilson", StartingCash = 10000m, CurrentCash = 9200m },
                    new { Username = "investor_pro", Email = "pro@example.com", FirstName = "David", LastName = "Brown", StartingCash = 10000m, CurrentCash = 11800m },
                    new { Username = "market_master", Email = "master@example.com", FirstName = "Lisa", LastName = "Garcia", StartingCash = 10000m, CurrentCash = 7600m },
                    new { Username = "bull_market", Email = "bull@example.com", FirstName = "Tom", LastName = "Miller", StartingCash = 10000m, CurrentCash = 13400m },
                    new { Username = "bear_defender", Email = "bear@example.com", FirstName = "Emma", LastName = "Taylor", StartingCash = 10000m, CurrentCash = 8900m }
                };

                foreach (var user in sampleUsers) {
                    var hashedPassword = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8"; // "password" hashed
                    
                    await connection.ExecuteAsync(@"
                        INSERT INTO users (username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance)
                        VALUES (@Username, @Email, @PasswordHash, @FirstName, @LastName, @StartingCash, @CurrentCash)",
                        new {
                            user.Username,
                            user.Email,
                            PasswordHash = hashedPassword,
                            user.FirstName,
                            user.LastName,
                            user.StartingCash,
                            user.CurrentCash
                        });
                }

                _logger.LogInformation("Successfully seeded {Count} sample users", sampleUsers.Length);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error seeding sample data");
            }
        }
    }
}

