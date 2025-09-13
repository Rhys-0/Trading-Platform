using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Services {
    internal class PortfolioService {
        private readonly Stocks _stocks;

        public PortfolioService(Stocks stocks) {
            _stocks = stocks;
        }

        /// <summary>
        /// Updates the value of a user's portfolio based on the current stock prices.
        /// </summary>
        /// <param name="user"></param>
        /// <remarks>This method does not update the database!</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        internal void UpdateUserPortfolio(IUser user) {
            if(user.Portfolio == null || user.Portfolio.Positions == null) {
                throw new InvalidOperationException("User does not have a portfolio to update, ensure it is loaded first.");
            }

            decimal value = 0.00m;
            foreach (var position in user.Portfolio.Positions.Values) {
                try {
                    value += (position.TotalQuantity * _stocks.StockList[position.StockSymbol].Price);
                } catch (KeyNotFoundException) {
                    throw new KeyNotFoundException("Invalid stock symbol, check if it is correct and it is in the Stocks constructor.");
                }
            }
            user.Portfolio.Value = value;
        }
    }
}
