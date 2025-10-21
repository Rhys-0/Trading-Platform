using TradingApp.Data.Interfaces;
using TradingApp.Models;

namespace TradingApp.Data {
    public class AuthenticationService : IAuthenticationService {
        private readonly ILoginManager _loginManager;
        private User? _currentUser;

        public AuthenticationService(ILoginManager loginManager) {
            _loginManager = loginManager;
        }

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public async Task<bool> LoginAsync(string usernameOrEmail, string password) {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password)) {
                return false;
            }

            try {
                string passwordHash = PasswordHasher.ComputeSha256Hash(password);
                User? user = null;
                
                // Try to login with email first if input contains @
                if (usernameOrEmail.Contains('@', StringComparison.Ordinal)) {
                    user = await _loginManager.RetrieveUser(usernameOrEmail, passwordHash);
                } else {
                    user = await _loginManager.RetrieveUserByUsername(usernameOrEmail, passwordHash);
                }

                if (user != null) {
                    _currentUser = user;
                    return true;
                }

                return false;
            } catch (Exception ex) {
                Console.WriteLine($"Database connection error: {ex.Message}");
                return false;
            }
        }

        public Task LogoutAsync() {
            _currentUser = null;
            return Task.CompletedTask;
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName) {
            try {
                // Hash the password before storing it
                string passwordHash = PasswordHasher.ComputeSha256Hash(password);
                
                // Try real database registration first
                bool success = await _loginManager.AddUser(username, email, passwordHash, firstName, lastName);
                
                if (success) {
                    // Registration successful, now login the user
                    return await LoginAsync(username, password);
                }
                
                return false;
            } catch (Exception ex) {
                Console.WriteLine($"Database registration error: {ex.Message}");
                return false;
            }
        }

    }
}
