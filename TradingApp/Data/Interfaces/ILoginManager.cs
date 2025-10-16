using TradingApp.Models;
using Npgsql;

namespace TradingApp.Data.Interfaces {
    public interface ILoginManager {
        public Task<User?> RetrieveUser(string email, string hashedPassword);
        public Task<User?> RetrieveUserByUsername(string username, string hashedPassword);
        public Task<bool> AddUser(string username, string email, string passwordHash, string firstName, string lastName);
        public Task<NpgsqlConnection> GetConnectionAsync();
    }
}
