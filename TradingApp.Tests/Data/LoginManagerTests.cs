using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Data;
using Xunit;

/* ----------------------------------------------------------------------------
 *  All tests in this file require docker to be installed and running to work!
 * ---------------------------------------------------------------------------- */

namespace TradingApp.Tests.Data {
    public class LoginManagerTests : IClassFixture<MockDatabase>{
        private readonly MockDatabase _db;
        public LoginManagerTests(MockDatabase db) => _db = db;

        [Fact]
        public async Task RetrieveUser_WithValidParams_ShouldRetrieveUser() {
            // Arrange
            await _db.ResetDatabaseAsync();

            await _db.ExecuteAsync(@"
                INSERT INTO users (username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance)
                VALUES ('testuser', 'test@email.com', 'hashedPass', 'Test', 'User', 100.00, 100.00);");

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            

            // Act
            var user = await loginManager.RetrieveUser("test@email.com", "hashedPass");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user!.Username);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("User", user.LastName);
            Assert.Equal(100.00m, user.StartingCashBalance);
            Assert.Equal(100.00m, user.CurrentCashBalance);
        }

        [Fact]
        public async Task RetrieveUser_WithInvalidEmail_ShouldReturnNullUser() {
            // Arrange
            await _db.ResetDatabaseAsync();

            await _db.ExecuteAsync(@"
                INSERT INTO users (username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance)
                VALUES ('testuser', 'test@email.com', 'hashedPass', 'Test', 'User', 100.00, 100.00);");

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));


            // Act
            var user = await loginManager.RetrieveUser("test2@email.com", "hashedPass");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task RetrieveUser_WithInvalidPassword_ShouldReturnNullUser() {
            // Arrange
            await _db.ResetDatabaseAsync();

            await _db.ExecuteAsync(@"
                INSERT INTO users (username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance)
                VALUES ('testuser', 'test@email.com', 'hashedPass', 'Test', 'User', 100.00, 100.00);");

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));


            // Act
            var user = await loginManager.RetrieveUser("test@email.com", "hashedpass");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task AddUser_WithValidParams_ShouldReturnTrue() {
            // Arrange
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));

            // Act
            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");

            // Assert
            Assert.True(response);
        }

        [Fact]
        public async Task AddUser_WithDuplicateEmail_ShouldReturnFalse() {
            // Arrange
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));

            // Act
            var response1 = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var response2 = await loginManager.AddUser("testuser2", "test@email.com", "hashedPass2", "Test2", "User2");

            // Assert
            Assert.True(response1);
            Assert.False(response2);    
        }

        [Fact]
        public async Task RetrieveUser_AddedByAddUser_ShouldRetrieveUser() {
            // Arrange
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));

            // Act
            var response1 = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await loginManager.RetrieveUser("test@email.com", "hashedPass");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user!.Username);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("User", user.LastName);

            Assert.True(response1);
        }
    }
}
