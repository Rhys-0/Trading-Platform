using TradingApp.Data;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using Npgsql;
using Xunit;

namespace TradingApp.Tests {
    public class AuthenticationTests {
        private readonly MockLoginManager _mockLoginManager;
        private readonly AuthenticationService _authService;

        public AuthenticationTests() {
            _mockLoginManager = new MockLoginManager();
            _authService = new AuthenticationService(_mockLoginManager);
        }

        [Fact]
        public async Task LoginAsync_WithValidEmail_ShouldReturnTrue() {
            // Arrange
            var email = "test@email.com";
            var password = "password123";
            
            // Act
            var result = await _authService.LoginAsync(email, password);
            
            // Assert
            Assert.True(result);
            Assert.NotNull(_authService.CurrentUser);
            Assert.Equal(email, _authService.CurrentUser.Email);
            Assert.True(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task LoginAsync_WithValidUsername_ShouldReturnTrue() {
            // Arrange
            var username = "testuser";
            var password = "password123";
            
            // Act
            var result = await _authService.LoginAsync(username, password);
            
            // Assert
            Assert.True(result);
            Assert.NotNull(_authService.CurrentUser);
            Assert.Equal(username, _authService.CurrentUser.Username);
            Assert.True(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFalse() {
            // Arrange
            var email = "invalid@email.com";
            var password = "wrongpassword";
            
            // Act
            var result = await _authService.LoginAsync(email, password);
            
            // Assert
            Assert.False(result);
            Assert.Null(_authService.CurrentUser);
            Assert.False(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task LoginAsync_WithEmptyCredentials_ShouldReturnFalse() {
            // Arrange
            var email = "";
            var password = "";
            
            // Act
            var result = await _authService.LoginAsync(email, password);
            
            // Assert
            Assert.False(result);
            Assert.Null(_authService.CurrentUser);
            Assert.False(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ShouldReturnTrue() {
            // Arrange
            var username = "newuser";
            var email = "newuser@email.com";
            var password = "password123"; // Use same password as mock expects
            var firstName = "New";
            var lastName = "User";
            
            // Act
            var result = await _authService.RegisterAsync(username, email, password, firstName, lastName);
            
            // Assert
            Assert.True(result);
            Assert.NotNull(_authService.CurrentUser);
            Assert.Equal(username, _authService.CurrentUser.Username);
            Assert.Equal(email, _authService.CurrentUser.Email);
            Assert.Equal(firstName, _authService.CurrentUser.FirstName);
            Assert.Equal(lastName, _authService.CurrentUser.LastName);
            Assert.True(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnFalse() {
            // Arrange
            var username = "anotheruser";
            var email = "test@email.com"; // Already exists in mock data
            var password = "password123";
            var firstName = "Another";
            var lastName = "User";
            
            // Act
            var result = await _authService.RegisterAsync(username, email, password, firstName, lastName);
            
            // Assert
            Assert.False(result);
            Assert.Null(_authService.CurrentUser);
            Assert.False(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ShouldReturnFalse() {
            // Arrange
            var username = "testuser"; // Already exists in mock data
            var email = "different@email.com";
            var password = "password123";
            var firstName = "Different";
            var lastName = "User";
            
            // Act
            var result = await _authService.RegisterAsync(username, email, password, firstName, lastName);
            
            // Assert
            Assert.False(result);
            Assert.Null(_authService.CurrentUser);
            Assert.False(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task LogoutAsync_ShouldClearCurrentUser() {
            // Arrange
            await _authService.LoginAsync("test@email.com", "password123");
            Assert.True(_authService.IsAuthenticated);
            
            // Act
            await _authService.LogoutAsync();
            
            // Assert
            Assert.Null(_authService.CurrentUser);
            Assert.False(_authService.IsAuthenticated);
        }

        [Fact]
        public async Task LoginAsync_ThenRegister_ShouldMaintainCorrectUser() {
            // Arrange
            await _authService.LoginAsync("test@email.com", "password123");
            var firstUser = _authService.CurrentUser;
            
            // Act
            await _authService.RegisterAsync("newuser2", "newuser2@email.com", "password123", "New", "User");
            var secondUser = _authService.CurrentUser;
            
            // Assert
            Assert.NotEqual(firstUser?.Username, secondUser?.Username);
            Assert.Equal("newuser2", secondUser?.Username);
        }
    }

    // Mock implementation for testing
    public class MockLoginManager : ILoginManager {
        private readonly List<User> _users = new() {
            new User(1, "testuser", "test@email.com", "Test", "User", 10000m, 10000m),
            new User(2, "demo", "demo@email.com", "Demo", "User", 15000m, 12000m)
        };

        public async Task<User?> RetrieveUser(string email, string hashedPassword) {
            await Task.Delay(10); // Simulate async operation
            return _users.FirstOrDefault(u => u.Email == email && HashPassword(GetPasswordForUser(u)) == hashedPassword);
        }

        public async Task<User?> RetrieveUserByUsername(string username, string hashedPassword) {
            await Task.Delay(10); // Simulate async operation
            return _users.FirstOrDefault(u => u.Username == username && HashPassword(GetPasswordForUser(u)) == hashedPassword);
        }

        public async Task<bool> AddUser(string username, string email, string passwordHash, string firstName, string lastName) {
            await Task.Delay(10); // Simulate async operation
            
            // Check if user already exists
            if (_users.Any(u => u.Email == email || u.Username == username)) {
                return false;
            }

            var newUser = new User(_users.Count + 1, username, email, firstName, lastName, 10000m, 10000m);
            _users.Add(newUser);
            return true;
        }

        public async Task<NpgsqlConnection> GetConnectionAsync() {
            await Task.Delay(10); // Simulate async operation
            // Return a mock connection - in real tests this would be handled by MockDatabase
            throw new NotImplementedException("Use MockDatabase for connection testing");
        }

        private string HashPassword(string password) {
            return PasswordHasher.ComputeSha256Hash(password);
        }

        private string GetPasswordForUser(User user) {
            // Return different passwords for different users in mock data
            return user.Username switch {
                "testuser" => "password123",
                "demo" => "password123",
                _ => "password123" // Default password for newly registered users
            };
        }
    }
}
