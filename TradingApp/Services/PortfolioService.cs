using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using TradingApp.Models;


namespace TradingApp.Services
{
    internal class PortfolioService
    {
        private readonly Stocks _stocks;

        public PortfolioService(Stocks stocks)
        {
            _stocks = stocks;
        }


        /// <summary>
        /// Event triggered when a user's portfolio is updated due to a trade execution.
        /// Subscribe to this event to receive notifications when trades occur.
        /// </summary>
        public event Action? OnPortfolioChanged;


        /// <summary>
        /// Notifies all subscribers that a portfolio has been updated.
        /// Call this method after executing a buy or sell trade to trigger UI updates.
        /// </summary>
        public void NotifyPortfolioChanged()
        {
            OnPortfolioChanged?.Invoke();
        }

        /// <summary>
        /// Updates the value of a user's portfolio based on the current stock prices.
        /// </summary>
        /// <param name="user"></param>
        /// <remarks>This method does not update the database!</remarks>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        internal void UpdateUserPortfolio(User user)
        {
            if (user is null)
            {
                return;
            }
            if (user.Portfolio == null || user.Portfolio.Positions == null)
            {
                throw new InvalidOperationException("User does not have a portfolio to update, ensure it is loaded first.");
            }

            decimal value = 0.00m;
            foreach (var position in user.Portfolio.Positions.Values)
            {
                if (_stocks.StockList.TryGetValue(position.StockSymbol, out var stockInfo)) {
                    value += position.TotalQuantity * stockInfo.Price;
                } else {
                    throw new KeyNotFoundException(
                        $"Stock symbol '{position.StockSymbol}' not found in current price list.");
                }
            }
            user.Portfolio.Value = value;
            user.Portfolio.NetProfit = (user.CurrentCashBalance + value) - user.StartingCashBalance;
            user.Portfolio.PercentageReturn = user.Portfolio.NetProfit / user.StartingCashBalance;
        }
    }
}