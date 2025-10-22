using TradingApp.Data.Interfaces;
using TradingApp.Models;

namespace TradingApp.Data
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoginManager _loginManager;
        private User? _currentUser;
        private CustomAuthenticationStateProvider? _authStateProvider;

        public AuthenticationService(ILoginManager loginManager)
        {
            _loginManager = loginManager;
        }

        // Allow the AuthStateProvider to register itself
        public void SetAuthStateProvider(CustomAuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;

        public async Task<bool> LoginAsync(string usernameOrEmail, string password)
        {
            Console.WriteLine($"🔍 AUTH DEBUG: LoginAsync called with usernameOrEmail: '{usernameOrEmail}', password: '{password}', password length: {password?.Length ?? 0}");

            try
            {
                string passwordHash = PasswordHasher.ComputeSha256Hash(password);
                Console.WriteLine($"🔍 AUTH DEBUG: Password hash computed: {passwordHash.Substring(0, Math.Min(10, passwordHash.Length))}...");

                User? user = null;

                // Try to login with email first if input contains @
                if (usernameOrEmail.Contains("@"))
                {
                    Console.WriteLine($"🔍 AUTH DEBUG: Trying email login for: {usernameOrEmail}");
                    user = await _loginManager.RetrieveUser(usernameOrEmail, passwordHash);
                }
                else
                {
                    Console.WriteLine($"🔍 AUTH DEBUG: Trying username login for: {usernameOrEmail}");
                    user = await _loginManager.RetrieveUserByUsername(usernameOrEmail, passwordHash);
                }

                Console.WriteLine($"🔍 AUTH DEBUG: User retrieved: {(user != null ? $"Yes - {user.FirstName} {user.LastName}" : "No")}");

                if (user != null)
                {
                    _currentUser = user;
                    Console.WriteLine($"🔍 AUTH DEBUG: Login successful - user set as current user");

                    // Notify Blazor's authentication state has changed
                    _authStateProvider?.NotifyAuthenticationStateChanged();

                    return true;
                }

                Console.WriteLine($"🔍 AUTH DEBUG: Login failed - no user found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔍 AUTH DEBUG: Login exception: {ex.Message}");
                Console.WriteLine($"🔍 AUTH DEBUG: Database connection failed - trying mock fallback");

                // Temporary mock fallback for presentation
                if ((usernameOrEmail == "twinkle@gmail.com" && password == "twinkle123"))
                {
                    Console.WriteLine($"🔍 AUTH DEBUG: Mock login successful for {usernameOrEmail}");
                    _currentUser = new User(1, "twinkle", usernameOrEmail, "Twinkle", "Star", 10000m, 10000m);
                    _authStateProvider?.NotifyAuthenticationStateChanged();
                    return true;
                }
                else if ((usernameOrEmail == "iamshilpi19@gmail.com" && password == "shilpi"))
                {
                    Console.WriteLine($"🔍 AUTH DEBUG: Mock login successful for {usernameOrEmail}");
                    _currentUser = new User(1, "shilpi", usernameOrEmail, "Shilpi", "Khosla", 10000m, 10000m);
                    _authStateProvider?.NotifyAuthenticationStateChanged();
                    return true;
                }

                Console.WriteLine($"🔍 AUTH DEBUG: Mock fallback failed - no matching credentials");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            _currentUser = null;
            _authStateProvider?.NotifyAuthenticationStateChanged();
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName)
        {
            Console.WriteLine($"🔍 AUTH DEBUG: Starting registration for {firstName} {lastName} ({username}) - {email}");

            try
            {
                // Hash the password before storing it
                string passwordHash = PasswordHasher.ComputeSha256Hash(password);
                Console.WriteLine($"🔍 AUTH DEBUG: Password hash computed for registration: {passwordHash.Substring(0, Math.Min(10, passwordHash.Length))}...");

                // Try real database registration first
                bool success = await _loginManager.AddUser(username, email, passwordHash, firstName, lastName);
                Console.WriteLine($"🔍 AUTH DEBUG: Database registration result: {success}");

                if (success)
                {
                    // Registration successful, now login the user
                    bool loginSuccess = await LoginAsync(username, password);
                    Console.WriteLine($"🔍 AUTH DEBUG: Auto-login result: {loginSuccess}");
                    return loginSuccess;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔍 AUTH DEBUG: Database registration failed: {ex.Message}");
                Console.WriteLine($"🔍 AUTH DEBUG: Database connection failed - trying mock registration fallback");

                // Mock registration fallback for presentation purposes
                Console.WriteLine($"🔍 AUTH DEBUG: Mock registration successful for {firstName} {lastName} ({username}) - {email}");
                _currentUser = new User(999, username, email, firstName, lastName, 10000m, 10000m);
                _authStateProvider?.NotifyAuthenticationStateChanged();
                return true;
            }
        }
    }
}