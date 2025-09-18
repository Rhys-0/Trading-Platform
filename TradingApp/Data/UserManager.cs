using Dapper;
using System.Data;
using System.Transactions;
using TradingApp.Data.Interfaces;
using TradingApp.Models;


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

        public async Task<bool> UpdateUser(User user) {
            using var connection = await _connection.CreateConnectionAsync();
            int rowsAffected = await connection.ExecuteAsync(
                "UPDATE users SET " +
                "email = @Email, " +
                "first_name = @FirstName, " +
                "last_name = @LastName, " +
                "current_cash_balance = @CurrentCashBalance " +
                "WHERE username = @Username",
                new {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.CurrentCashBalance,
                    user.Username
                });

            return (rowsAffected == 1);
        }

        public async Task<bool> LoadUserPortfolio(User user) {
            using var connection = await _connection.CreateConnectionAsync();
            // Load the portfolio
            var portfolio = await connection.QueryFirstOrDefaultAsync<Portfolio>(
                "SELECT portfolio_id AS portfolioId, " +
                "value, " +
                "net_profit AS netProfit, " +
                "percentage_return AS percentageReturn " +
                "FROM portfolio " +
                "WHERE user_id = @UserId",
                new { UserId = user.Id });

            if (portfolio is null) return false;

            // Load positions
            var positions = (await connection.QueryAsync<Position>(
                "SELECT position_id AS positionId, " +
                "stock_symbol AS stockSymbol, " +
                "total_quantity AS totalQuantity " +
                "FROM position " +
                "WHERE portfolio_id = @PortfolioId",
                new { portfolio.PortfolioId })).ToList();

            portfolio.Positions = [];

            // Load purchase lots for each position
            foreach (Position position in positions) {
                var purchaseLots = (await connection.QueryAsync<PurchaseLot>(
                    "SELECT purchase_lot_id AS purchaseLotId, " +
                    "quantity, " +
                    "purchase_price AS purchasePrice, " +
                    "purchase_date AS purchaseDate " +
                    "FROM purchase_lot " +
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

        public async Task LoadUserTrades(User user) {
            using var connection = await _connection.CreateConnectionAsync();
            var trades = (await connection.QueryAsync<Trade>(
                "SELECT trade_id AS TradeId, " +
                "trade_type AS TradeType, " +
                "stock_symbol AS StockSymbol, " +
                "quantity AS Quantity, " +
                "price AS Price, " +
                "time AS Time " +
                "FROM trade " +
                "WHERE user_id = @UserId",
                new { UserId = user.Id })).ToList();

            user.Trades = trades;
        }

        /// <summary>
        /// Log a trade to the database and add it to the user's trade history in memory.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stockSymbol"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="tradeType"></param>
        /// <exception cref="ArgumentException">Thrown if the trade type is invalid</exception>"
        public async Task<long> LogTrade(User user, string stockSymbol, int quantity, decimal price, string tradeType) {
            using var connection = await _connection.CreateConnectionAsync();

            if (tradeType != "BUY" && tradeType != "SELL") {
                throw new ArgumentException("Invalid trade type. Must be 'BUY' or 'SELL'.");
            }   

            (var tradeId, var time) = await connection.QueryFirstOrDefaultAsync<(long, DateTime)>(
                "INSERT INTO trade (user_id, stock_symbol, quantity, price, trade_type) " +
                "VALUES (@UserId, @StockSymbol, @Quantity, @Price, @TradeType) " +
                "RETURNING trade_id, time",
                new {
                    UserId = user.Id,
                    StockSymbol = stockSymbol,
                    Quantity = quantity,
                    Price = price,
                    TradeType = tradeType,
                });

            user.Trades ??= [];
            user.Trades.Add(new Trade(tradeId, tradeType, stockSymbol, quantity, price, time));
            return tradeId;
        }

        /// <summary>
        /// Updates the position and purchaseLots in the database to reflect changes made to it in memory. 
        /// PurchaseLots are updated due to having a tightly coupled relationship with positions.
        /// </summary>
        /// <param name="position">The position to update</param>
        /// <param name="portfolioId">The id of the portfolio the position belongs to</param>
        /// <exception cref="InvalidOperationException">Thrown if the position could not be updated</exception>"
        public async Task UpdatePosition(Position position, long portfolioId) {
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

        public async Task<bool> OpenPosition(Position position, long portfolioId, long tradeId) {
            using var connection = await _connection.CreateConnectionAsync();

            // Start a transaction to ensure data integrity
            using var transaction = connection.BeginTransaction();
            try {
                // Insert the position and purchase lots
                position.PositionId = await connection.QuerySingleOrDefaultAsync<long>(
                    "INSERT INTO position (portfolio_id, stock_symbol, total_quantity) " +
                    "VALUES (@portfolioId, @StockSymbol, @TotalQuantity) " +
                    "RETURNING position_id",
                    new {
                        portfolioId,
                        position.StockSymbol,
                        position.TotalQuantity,
                    }, transaction: transaction
                );

                foreach (var lot in position.PurchaseLots!) {
                    lot.PurchaseLotId = await connection.QuerySingleOrDefaultAsync<long>(
                        "INSERT INTO purchase_lot (position_id, trade_id, quantity, purchase_price, purchase_date) " +
                        "VALUES (@PositionId, @TradeId, @Quantity, @PurchasePrice, @PurchaseDate) " +
                        "RETURNING purchase_lot_id",
                        new {
                            position.PositionId,
                            TradeId = tradeId,
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

        public async Task<bool> ClosePosition(Position position, long portfolioId) {
            using var connection = await _connection.CreateConnectionAsync();

            // Start a transaction to ensure data integrity
            using var transaction = connection.BeginTransaction();
            try {
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
