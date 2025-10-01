using TradingApp.Models;

namespace TradingApp.Data.Interfaces {
    public interface IAuthenticationService {
        User? CurrentUser { get; }
        bool IsAuthenticated { get; }
        Task<bool> LoginAsync(string usernameOrEmail, string password);
        Task LogoutAsync();
        Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName);
    }
}
