using TradingApp.Models;


namespace TradingApp.Data.Interfaces {
    internal interface IUserManager {
        public Task<User?> GetUser(string username);
        public Task<bool> UpdateUser(User user);
        public Task<bool> LoadUserPortfolio(User user);
        public Task LoadUserTrades(User user);
        public Task LogTrade(User user, string stockSymbol, int quantity, decimal price, string tradeType);
    }
}
