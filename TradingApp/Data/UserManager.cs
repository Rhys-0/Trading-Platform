using Dapper;
using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Data {
    internal class UserManager {
        private readonly DatabaseConnection _connection;
        public UserManager(DatabaseConnection connection) {
            _connection = connection;
        }
        internal async Task<User?> GetUser(string username) {
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

        internal async Task<bool> UpdateUser(IUser user) {
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

        internal async Task<bool> LoadUserPortfolio(IUser user) {
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

            if (portfolio == null) return false;

            // Load positions
            var positions = (await connection.QueryAsync<IPosition>(
                "SELECT position_id AS PositionId, " +
                "stock_symbol AS StockSymbol, " +
                "quantity AS Quantity, " +
                "average_price AS AveragePrice " +
                "FROM portfolio_positions " +
                "WHERE portfolio_id = @PortfolioId",
                new { portfolio.PortfolioId })).ToList();

            if (positions == null) return false;

            portfolio.Positions = positions;

            // Load purchase lots for each position
            foreach (IPosition position in positions) {
                var purchaseLots = (await connection.QueryAsync<IPurchaseLot>(
                    "SELECT lot_id AS LotId, " +
                    "quantity AS Quantity, " +
                    "purchase_price AS PurchasePrice, " +
                    "purchase_date AS PurchaseDate " +
                    "FROM position_purchase_lots " +
                    "WHERE position_id = @PositionId",
                    new { position.PositionId })).ToList();
                if (purchaseLots == null) return false;
                position.PurchaseLots = purchaseLots;
            }

            // Update the portfolio of the user
            user.Portfolio = portfolio;
            return true;
        }

        internal async Task<bool> LoadUserTrades(User user) {
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

            if (trades == null) return false;
            user.Trades = trades;
            return true;
        }

        internal async Task<bool> SellUserStocks(User user, string stockSymbol, int quantity, decimal price) {
            using var connection = await _connection.CreateConnectionAsync();
            return false;
        }

        internal async Task<bool> BuyUserStocks(User user, string stockSymbol, int quantity, decimal price) {
            using var connection = await _connection.CreateConnectionAsync();
            return false;
        }
    }
}
