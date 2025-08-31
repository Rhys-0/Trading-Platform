using TradingApp.Models;

namespace TradingApp.Data.Interfaces {
    internal interface ILoginManager {
        public Task<User?> RetrieveUser(string email, string hashedPassword);
        public Task<bool> AddUser(string username, string email, string passwordHash, string firstName, string lastName);
    }
}
