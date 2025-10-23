using Npgsql;
using Dapper;
using System.Data;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Data {
    public class LeaderboardService : ILeaderboardService {
        private readonly DatabaseConnection _connection;
        private readonly ILogger<LeaderboardService> _logger;
        private readonly Stocks _stocks;

        public LeaderboardService(DatabaseConnection connection, ILogger<LeaderboardService> logger, Stocks stocks) {
            _connection = connection;
            _logger = logger;
            _stocks = stocks;
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync() {
            try {
                // Try to get data from database first
                using var connection = await _connection.CreateConnectionAsync();
                
                // Get all users with their basic info
                var usersQuery = @"
                    SELECT 
                        u.user_id,
                        u.username,
                        u.first_name,
                        u.last_name,
                        u.starting_cash_balance,
                        u.current_cash_balance
                    FROM users u";

                var users = await connection.QueryAsync<dynamic>(usersQuery);
                var leaderboard = new List<LeaderboardEntry>();

                foreach (var user in users) {
                    // Calculate total portfolio value including stock positions
                    decimal totalValue = await CalculateTotalPortfolioValue(connection, (int)user.user_id, user.current_cash_balance);
                    decimal netProfit = totalValue - user.starting_cash_balance;
                    decimal percentageReturn = user.starting_cash_balance > 0 
                        ? (netProfit / user.starting_cash_balance) * 100 
                        : 0;

                    leaderboard.Add(new LeaderboardEntry {
                        UserId = (int)user.user_id,
                        Username = user.username,
                        FirstName = user.first_name,
                        LastName = user.last_name,
                        StartingBalance = user.starting_cash_balance,
                        CurrentBalance = totalValue,
                        NetProfit = netProfit,
                        PercentageReturn = percentageReturn,
                        LastUpdated = DateTime.Now
                    });
                }

                // Sort by percentage return and add ranks
                leaderboard = leaderboard.OrderByDescending(entry => entry.PercentageReturn).ToList();
                for (int i = 0; i < leaderboard.Count; i++) {
                    leaderboard[i].Rank = i + 1;
                }

                return leaderboard;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error retrieving leaderboard from database");
                return new List<LeaderboardEntry>();
            }
        }

        private async Task<decimal> CalculateTotalPortfolioValue(IDbConnection connection, long userId, decimal cashBalance) {
            try {
                // Get all positions for this user
                var positionsQuery = @"
                    SELECT 
                        p.stock_symbol,
                        p.total_quantity
                    FROM position p
                    INNER JOIN portfolio pf ON p.portfolio_id = pf.portfolio_id
                    WHERE pf.user_id = @UserId";

                var positions = await connection.QueryAsync<dynamic>(positionsQuery, new { UserId = userId });
                
                decimal totalStockValue = 0;
                
                foreach (var position in positions) {
                    string stockSymbol = position.stock_symbol;
                    int quantity = position.total_quantity;
                    
                    // Get current price from Stocks service
                    if (_stocks.StockList.TryGetValue(stockSymbol, out var stock)) {
                        decimal currentPrice = stock.Price;
                        decimal positionValue = quantity * currentPrice;
                        totalStockValue += positionValue;
                        
                        _logger.LogDebug("Position: {Symbol} x {Quantity} @ ${Price} = ${Value}", 
                            stockSymbol, quantity, currentPrice, positionValue);
                    } else {
                        _logger.LogWarning("Stock symbol {Symbol} not found in current stock list", stockSymbol);
                    }
                }
                
                decimal totalPortfolioValue = cashBalance + totalStockValue;
                _logger.LogDebug("User {UserId}: Cash ${Cash} + Stocks ${Stocks} = Total ${Total}", 
                    userId, cashBalance, totalStockValue, totalPortfolioValue);
                
                return totalPortfolioValue;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error calculating portfolio value for user {UserId}", userId);
                return cashBalance; // Return just cash balance if calculation fails
            }
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
