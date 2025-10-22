using TradingApp.Data;
using TradingApp.Models;
using System.Security.Claims;

namespace TradingApp.Services
{
    internal class UserService
    {
        private readonly Stocks _Stocks;
        private readonly UserManager _UserManager;
        private readonly PortfolioService _PortfolioService;

        public UserService(Stocks stocks, UserManager userManager, PortfolioService portfolioService)
        {
            _Stocks = stocks;
            _UserManager = userManager;
            _PortfolioService = portfolioService;
        }

        /// <summary>
        /// Execute a sell trade for a user, updating the user's cash balance and portfolio accordingly in both memory and the database.
        /// </summary>
        public async Task<bool> ExecuteSellTrade(User user, string stockTicker, int quantity)
        {
            _Stocks.StockList.TryGetValue(stockTicker, out Stock? stock);
            if (stock == null)
            {
                throw new ArgumentException("Stock not found.");
            }

            if (user.Portfolio is null || user.Portfolio.Positions is null)
            {
                throw new ArgumentException("User portfolio not loaded.");
            }

            decimal totalCost = stock.Price * quantity;
            bool cashAdded = user.AddCash(totalCost);
            if (!cashAdded)
            {
                return false;
            }

            user.Portfolio.RemoveStocks(stockTicker, quantity);

            // Log the trade in the database
            await _UserManager.LogTrade(user, stockTicker, quantity, stock.Price, "SELL");
            await _UserManager.UpdateUser(user);

            // Updates Portfolio Value Immediately
            _PortfolioService.UpdateUserPortfolio(user);
            await _UserManager.UpdatePortfolio(user.Portfolio!);

            // Close the position if all stocks have been sold
            if (user.Portfolio.Positions[stockTicker].TotalQuantity == 0)
            {
                await _UserManager.ClosePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
                user.Portfolio.Positions.Remove(stockTicker);
                return true;
            }

            await _UserManager.UpdatePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);

            return true;
        }

        public async Task<bool> ExecuteBuyTrade(User user, string stockTicker, int quantity)
        {
            _Stocks.StockList.TryGetValue(stockTicker, out Stock? stock);
            if (stock == null)
            {
                throw new ArgumentException("Stock not found.");
            }
            if (user.Portfolio is null || user.Portfolio.Positions is null)
            {
                throw new ArgumentException("User portfolio not loaded.");
            }
            decimal totalCost = stock.Price * quantity;
            bool cashRemoved = user.RemoveCash(totalCost);
            if (!cashRemoved)
            {
                return false; // Not enough cash to execute the trade
            }

            user.Portfolio.AddStocks(stockTicker, quantity, stock.Price);

            // Log the trade
            var tradeId = await _UserManager.LogTrade(user, stockTicker, quantity, stock.Price, "BUY");
            await _UserManager.UpdateUser(user);

            // Updates Portfolio Value Immediately
            _PortfolioService.UpdateUserPortfolio(user);
            await _UserManager.UpdatePortfolio(user.Portfolio!);

            if (user.Portfolio.Positions[stockTicker].PurchaseLots!.Count == 1)
            {
                await _UserManager.OpenPosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId, tradeId);
                return true;
            }

            await _UserManager.UpdatePosition(user.Portfolio.Positions[stockTicker], user.Portfolio.PortfolioId);
            return true;
        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal?.Identity?.IsAuthenticated != true)
                return null;

            var username = userPrincipal.Identity.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            var user = await _UserManager.GetUser(username);
            if (user == null)
                return null;

            await _UserManager.LoadUserPortfolio(user);
            await _UserManager.LoadUserTrades(user);

            return user;
        }
    }
}