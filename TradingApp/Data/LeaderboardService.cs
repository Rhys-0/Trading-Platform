using Npgsql;
using Dapper;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Data {
    public class LeaderboardService : ILeaderboardService {
        private readonly DatabaseConnection _connection;
        private readonly ILogger<LeaderboardService> _logger;

        public LeaderboardService(DatabaseConnection connection, ILogger<LeaderboardService> logger) {
            _connection = connection;
            _logger = logger;
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync() {
            try {
                // Try to get data from database first
                using var connection = await _connection.CreateConnectionAsync();
                
                // Query to get all users with their current balances and calculate profit/loss
                var query = @"
                    SELECT 
                        u.user_id,
                        u.username,
                        u.first_name,
                        u.last_name,
                        u.starting_cash_balance,
                        u.current_cash_balance,
                        u.current_cash_balance as total_value,
                        (u.current_cash_balance - u.starting_cash_balance) as net_profit,
                        CASE 
                            WHEN u.starting_cash_balance > 0 
                            THEN ((u.current_cash_balance - u.starting_cash_balance) / u.starting_cash_balance) * 100
                            ELSE 0 
                        END as percentage_return,
                        NOW() as last_updated
                    FROM users u
                    ORDER BY percentage_return DESC";

                var entries = await connection.QueryAsync<LeaderboardEntry>(query);
                var leaderboard = entries.ToList();

                // Add rank numbers
                for (int i = 0; i < leaderboard.Count; i++) {
                    leaderboard[i].Rank = i + 1;
                }

                return leaderboard;
            } catch (Exception ex) {
                _logger.LogError(ex, "Database connection failed, returning mock data");
                
                // Return mock data when database is not available
                return GetMockLeaderboardData();
            }
        }

        private List<LeaderboardEntry> GetMockLeaderboardData() {
            return new List<LeaderboardEntry> {
                new() { UserId = 1, Username = "crypto_king", FirstName = "Alex", LastName = "Davis", StartingBalance = 10000m, CurrentBalance = 15200m, NetProfit = 5200m, PercentageReturn = 52.00m, Rank = 1, LastUpdated = DateTime.Now },
                new() { UserId = 2, Username = "bull_market", FirstName = "Tom", LastName = "Miller", StartingBalance = 10000m, CurrentBalance = 13400m, NetProfit = 3400m, PercentageReturn = 34.00m, Rank = 2, LastUpdated = DateTime.Now },
                new() { UserId = 3, Username = "trader_jane", FirstName = "Jane", LastName = "Smith", StartingBalance = 10000m, CurrentBalance = 12500m, NetProfit = 2500m, PercentageReturn = 25.00m, Rank = 3, LastUpdated = DateTime.Now },
                new() { UserId = 4, Username = "investor_pro", FirstName = "David", LastName = "Brown", StartingBalance = 10000m, CurrentBalance = 11800m, NetProfit = 1800m, PercentageReturn = 18.00m, Rank = 4, LastUpdated = DateTime.Now },
                new() { UserId = 5, Username = "day_trader", FirstName = "Sarah", LastName = "Wilson", StartingBalance = 10000m, CurrentBalance = 9200m, NetProfit = -800m, PercentageReturn = -8.00m, Rank = 5, LastUpdated = DateTime.Now },
                new() { UserId = 6, Username = "bear_defender", FirstName = "Emma", LastName = "Taylor", StartingBalance = 10000m, CurrentBalance = 8900m, NetProfit = -1100m, PercentageReturn = -11.00m, Rank = 6, LastUpdated = DateTime.Now },
                new() { UserId = 7, Username = "stock_guru", FirstName = "Mike", LastName = "Johnson", StartingBalance = 10000m, CurrentBalance = 8750m, NetProfit = -1250m, PercentageReturn = -12.50m, Rank = 7, LastUpdated = DateTime.Now },
                new() { UserId = 8, Username = "market_master", FirstName = "Lisa", LastName = "Garcia", StartingBalance = 10000m, CurrentBalance = 7600m, NetProfit = -2400m, PercentageReturn = -24.00m, Rank = 8, LastUpdated = DateTime.Now }
            };
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardSortedByGainsAsync() {
            try {
                var leaderboard = await GetLeaderboardAsync();
                return leaderboard
                    .Where(entry => entry.PercentageReturn >= 0)
                    .OrderByDescending(entry => entry.PercentageReturn)
                    .ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving leaderboard sorted by gains");
                return new List<LeaderboardEntry>();
            }
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardSortedByLossesAsync() {
            try {
                var leaderboard = await GetLeaderboardAsync();
                return leaderboard
                    .Where(entry => entry.PercentageReturn < 0)
                    .OrderBy(entry => entry.PercentageReturn) // Ascending (most negative first)
                    .ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving leaderboard sorted by losses");
                return new List<LeaderboardEntry>();
            }
        }

        public async Task<LeaderboardEntry?> GetUserRankAsync(int userId) {
            try {
                var leaderboard = await GetLeaderboardAsync();
                return leaderboard.FirstOrDefault(entry => entry.UserId == userId);
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving user rank for user {UserId}", userId);
                return null;
            }
        }

        public async Task<List<LeaderboardEntry>> GetTopPerformersAsync(int count = 10) {
            try {
                var leaderboard = await GetLeaderboardAsync();
                return leaderboard
                    .Take(count)
                    .ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving top performers");
                return new List<LeaderboardEntry>();
            }
        }

        public async Task<List<LeaderboardEntry>> GetWorstPerformersAsync(int count = 10) {
            try {
                var leaderboard = await GetLeaderboardAsync();
                return leaderboard
                    .OrderBy(entry => entry.PercentageReturn)
                    .Take(count)
                    .ToList();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving worst performers");
                return new List<LeaderboardEntry>();
            }
        }
    }
}
