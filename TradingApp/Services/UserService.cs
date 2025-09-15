using TradingApp.Data;
using TradingApp.Models;

namespace TradingApp.Services {
    internal class UserService {
        private readonly Stocks _Stocks;
        private readonly UserManager _UserManager;

        public UserService(Stocks stocks, UserManager userManager) {
            _Stocks = stocks;
            _UserManager = userManager;
        }

        /// <summary>
        /// Execute a sell trade for a user, updating the user's cash balance and portfolio accordingly in both memory and the database.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stockTicker"></param>
        /// <param name="quantity"></param>
        /// <returns>True if the operation succeeds, false otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if the stock is not found or the portfolio is not loaded.</exception>
        public async Task<bool> ExecuteSellTrade(User user, string stockTicker, int quantity) {
            _Stocks.StockList.TryGetValue(stockTicker, out Stock? stock);
            if (stock == null) {
                throw new ArgumentException("Stock not found.");
            }

            if (user.Portfolio is null || user.Portfolio.Positions is null) {
                throw new ArgumentException("User portfolio not loaded.");
            }

            decimal totalCost = stock.Price * quantity;
            bool cashAdded = user.AddCash(totalCost);
            if (!cashAdded) {
                return false; // Not enough cash to execute the trade
            }

            user.Portfolio.RemoveStocks(stockTicker, quantity);

            // Log the trade in the database
            await _UserManager.LogTrade(user, stockTicker, quantity, stock.Price, "SELL");
            await _UserManager.UpdateUser(user);

            // Close the position if all stocks have been sold
            if (user.Portfolio.Positions[stockTicker].TotalQuantity == 0) {
                await _UserManager.ClosePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
                user.Portfolio.Positions.Remove(stockTicker);
                return true;
            }

            await _UserManager.UpdatePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
            // Portfolio doesnt need to be updated in the DB as it is updated automatically via PortfolioUpdateService

            return true;
        }

        public async Task<bool> ExecuteBuyTrade(User user, string stockTicker, int quantity) {
            _Stocks.StockList.TryGetValue(stockTicker, out Stock? stock);
            if (stock == null) {
                throw new ArgumentException("Stock not found.");
            }
            if (user.Portfolio is null || user.Portfolio.Positions is null) {
                throw new ArgumentException("User portfolio not loaded.");
            }
            decimal totalCost = stock.Price * quantity;
            bool cashRemoved = user.RemoveCash(totalCost);
            if (!cashRemoved) {
                return false; // Not enough cash to execute the trade
            }
            
            user.Portfolio.AddStocks(stockTicker, quantity, stock.Price);

            // Log the trade
            await _UserManager.LogTrade(user, stockTicker, quantity, stock.Price, "BUY");
            await _UserManager.UpdateUser(user);

            if (user.Portfolio.Positions[stockTicker].PurchaseLots!.Count == 1) {
                await _UserManager.OpenPosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
                return true;
            }

            await _UserManager.UpdatePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
            return true;
        }

    }
}
