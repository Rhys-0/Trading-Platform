using Dapper;
using System.Data;
using System.Transactions;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Data {
    internal class UserManager : IUserManager {
        private readonly DatabaseConnection _connection;
        public UserManager(DatabaseConnection connection) {
            _connection = connection;
        }
        public async Task<User?> GetUser(string username) {
            using var connection = await _connection.CreateConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT user_id AS id, " +
                "username, " +
                "email, " +
                "first_name AS firstName, " +
                "last_name AS lastName, " +
                "starting_cash_balance AS startingCashBalance, " +
                "current_cash_balance AS currentCashBalance " +
                "FROM users " +
                "WHERE username = @Username",
                new { Username = username });
        }

        public async Task<bool> UpdateUser(IUser user) {
            using var connection = await _connection.CreateConnectionAsync();
            int rowsAffected = await connection.ExecuteAsync(
                "UPDATE users SET " +
                "email = @Email, " +
                "first_name = @FirstName, " +
                "last_name = @LastName, " +
                "current_cash_balance = @CurrentCashBalance " +
                "WHERE user_id = @Id",
                new {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.CurrentCashBalance,
                    user.Id
                });

            return (rowsAffected == 1);
        }

        public async Task<bool> LoadUserPortfolio(IUser user) {
            using var connection = await _connection.CreateConnectionAsync();
            // Load the portfolio
            var portfolio = await connection.QueryFirstOrDefaultAsync<IPortfolio>(
                "SELECT portfolio_id AS PortfolioId, " +
                "value AS Value, " +
                "net_profit AS NetProfit, " +
                "percentage_return AS PercentageReturn " +
                "FROM user_portfolios " +
                "WHERE user_id = @UserId",
                new { UserId = user.Id });

            if (portfolio is null) return false;

            // Load positions
            var positions = (await connection.QueryAsync<IPosition>(
                "SELECT position_id AS PositionId, " +
                "stock_symbol AS StockSymbol, " +
                "quantity AS Quantity, " +
                "average_price AS AveragePrice " +
                "FROM portfolio_positions " +
                "WHERE portfolio_id = @PortfolioId",
                new { portfolio.PortfolioId })).ToList();

            if (positions.Count == 0) return false;

            portfolio.Positions = [];

            // Load purchase lots for each position
            foreach ((_, IPosition position) in portfolio.Positions) {
                var purchaseLots = (await connection.QueryAsync<IPurchaseLot>(
                    "SELECT lot_id AS LotId, " +
                    "quantity AS Quantity, " +
                    "purchase_price AS PurchasePrice, " +
                    "purchase_date AS PurchaseDate " +
                    "FROM position_purchase_lots " +
                    "WHERE position_id = @PositionId",
                    new { position.PositionId })).ToList();
                if (purchaseLots.Count == 0) return false;
                position.PurchaseLots = purchaseLots;

                // Add the position to the portfolio dictionary
                portfolio.Positions.Add(position.StockSymbol, position);
            }

            // Update the portfolio of the user
            user.Portfolio = portfolio;
            return true;
        }

        public async Task<bool> LoadUserTrades(User user) {
            using var connection = await _connection.CreateConnectionAsync();
            var trades = (await connection.QueryAsync<ITrade>(
                "SELECT trade_id AS TradeId, " +
                "stock_symbol AS StockSymbol, " +
                "quantity AS Quantity, " +
                "price AS Price, " +
                "trade_type AS TradeType, " +
                "trade_date AS TradeDate " +
                "FROM trades " +
                "WHERE user_id = @UserId",
                new { UserId = user.Id })).ToList();

            if (trades.Count == 0) return false;
            user.Trades = trades;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stockSymbol"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public async Task<bool> LogSellTrade(User user, string stockSymbol, int quantity, decimal price) {
            using var connection = await _connection.CreateConnectionAsync();

            var query = await connection.ExecuteAsync(
                "INSERT INTO trades (user_id, stock_symbol, quantity, price, trade_type) " +
                "VALUES (@UserId, @StockSymbol, @Quantity, @Price, @TradeType)",
                new {
                    UserId = user.Id,
                    StockSymbol = stockSymbol,
                    Quantity = quantity,
                    Price = price,
                    TradeType = "SELL",
                });

            return false;
        }

        public async Task<bool> LogBuyTrade(User user, string stockSymbol, int quantity, decimal price) {
            using var connection = await _connection.CreateConnectionAsync();
            return false;
        }

        /// <summary>
        /// Updates the position and purchaseLots in the database to reflect changes made to it in memory. 
        /// PurchaseLots are updated due to having a tightly coupled relationship with positions.
        /// </summary>
        /// <param name="position">The position to update</param>
        /// <param name="portfolioId">The id of the portfolio the position belongs to</param>
        /// <exception cref="InvalidOperationException">Thrown if the position could not be updated</exception>"
        public async Task UpdatePosition(IPosition position, int portfolioId) {
            using var connection = await _connection.CreateConnectionAsync();

            // Start a transaction to ensure data integrity
            using var transaction = connection.BeginTransaction();

            // Rows are deleted then readded to account for new positions and removed purchase lots.
            try {
                // Delete existing purchase lots for the position
                await connection.ExecuteAsync(
                    "DELETE FROM purchase_lot " +
                    "WHERE position_id = @PositionId",
                    new {
                        position.PositionId
                    }, transaction: transaction
                );
                
                await connection.ExecuteAsync(
                    "DELETE FROM position " +
                    "WHERE position_id = @PositionId",
                    new {
                        position.PositionId
                    }, transaction: transaction
                );

                // Reinsert the position and purchase lots
                await connection.ExecuteAsync(
                    "INSERT INTO position (portfolio_id, stock_symbol, total_quantity) " +
                    "VALUES (@PortfolioId, @StockSymbol, @TotalQuantity) " +
                    "RETURNING position_id",
                    new {
                        PortfolioId = portfolioId,
                        position.StockSymbol,
                        position.TotalQuantity
                    }, transaction: transaction
                );

                foreach(var lot in position.PurchaseLots!) {
                    await connection.ExecuteAsync(
                        "INSERT INTO purchase_lot (position_id, quantity, purchase_price, purchase_date) " +
                        "VALUES (@PositionId, @Quantity, @PurchasePrice, @PurchaseDate)",
                        new {
                            position.PositionId,
                            lot.Quantity,
                            lot.PurchasePrice,
                            lot.PurchaseDate
                        }, transaction: transaction
                    );
                }

                // Commit the transaction if all operations succeed
                transaction.Commit();
            } catch(Exception) {
                transaction.Rollback();
                throw;
            }

        }

        public async Task<bool> ClosePosition(IPosition position, int portfolioId) {
            using var connection = await _connection.CreateConnectionAsync();

            // Start a transaction to ensure data integrity
            using var transaction = connection.BeginTransaction();
            try {
                // Insert the position and purchase lots
                position.PositionId = await connection.QuerySingleOrDefaultAsync<int>(
                    "INSERT INTO position (stock_symbol, total_quantity) " +
                    "VALUES (@StockSymbol, @TotalQuantity) " +
                    "RETURNING position_id",
                    new {
                        position.StockSymbol,
                        position.TotalQuantity,
                    }, transaction: transaction
                );

                foreach (var lot in position.PurchaseLots!) {
                    lot.PurchaseLotId = await connection.QuerySingleOrDefaultAsync(
                        "INSERT INTO purchase_lot (position_id, quantity, purchase_price, purchase_date) " +
                        "VALUES (@PositionId, @Quantity, @PurchasePrice, @PurchaseDate) " +
                        "RETURNING purchase_lot_id",
                        new {
                            position.PositionId,
                            lot.Quantity,
                            lot.PurchasePrice,
                            lot.PurchaseDate
                        }, transaction: transaction
                    );
                }

                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
                throw;
            }


            return true;
        }

        public async Task<bool> OpenPosition(IPosition position, int portfolioId) {
            using var connection = await _connection.CreateConnectionAsync();

            // Start a transaction to ensure data integrity
            using var transaction = connection.BeginTransaction();
            try {
                await connection.ExecuteAsync(
                    "INSERT FROM purchase_lot " +
                    "WHERE position_id = @PositionId",
                    new {
                        position.PositionId
                    }, transaction: transaction
                );
                await connection.ExecuteAsync(
                    "DELETE FROM position " +
                    "WHERE position_id = @PositionId",
                    new {
                        position.PositionId,
                    }, transaction: transaction
                );

                transaction.Commit();
            } catch (Exception) {
                transaction.Rollback();
                throw;
            }


            return true;
        }
    }
}
