using TradingApp.Models;
using TradingApp.Models.Interfaces;

namespace TradingApp.Data.Interfaces {
    internal interface IUserManager {
        public Task<User?> GetUser(string username);
        public Task<bool> UpdateUser(IUser user);
        public Task<bool> LoadUserPortfolio(IUser user);
        public Task<bool> LoadUserTrades(User user);
        public Task<bool> LogSellTrade(User user, string stockSymbol, int quantity, decimal price);
        public Task<bool> LogBuyTrade(User user, string stockSymbol, int quantity, decimal price);
    }
}
