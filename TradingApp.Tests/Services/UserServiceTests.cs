using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Services;
using Moq;
using TradingApp.Models;
using TradingApp.Data;
using Microsoft.Extensions.Configuration;
using TradingApp.Data.Interfaces;

namespace TradingApp.Tests.Services {
    public class UserServiceTests : IClassFixture<MockDatabase> {
        private readonly MockDatabase _db;
        public UserServiceTests(MockDatabase db) => _db = db;

        [Fact]
        public void Constructor_WithValidParams_ShouldInitaliseValues() {
            // Arrange
            var stocks = new Stocks();
            var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                            {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
            })
            .Build();
            var userManager = new UserManager(new DatabaseConnection(config));
            // Act
            var userService = new UserService(stocks, userManager);
            // Assert
            Assert.NotNull(userService);
        }

        [Fact]
        public async Task ExecuteBuyTrade_WithValidParams_ShouldExecuteSellTrade() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var portfolioReturn = await userManager.LoadUserPortfolio(user!);
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act
            var returnValue = await userService.ExecuteBuyTrade(user!, "AAPL", 1);

            // Assert
            Assert.True(returnValue);
            Assert.Single(user!.Trades!);
        }

        [Fact]
        public async Task ExecuteBuyTrade_WithInvalidAmount_ShouldReturnFalse() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var portfolioReturn = await userManager.LoadUserPortfolio(user!);
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act
            var returnValue = await userService.ExecuteBuyTrade(user!, "AAPL", -5);
            
            // Assert
            Assert.False(returnValue);
        }

        [Fact]
        public async Task ExecuteBuyTrade_WithInvalidStock_ShouldThrowException() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var portfolioReturn = await userManager.LoadUserPortfolio(user!);
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => userService.ExecuteBuyTrade(user!, "FAKESTOCK", 1));
        }

        [Fact]
        public async Task ExecuteBuyTrade_WithoutPortfolioLoaded_ShouldThrowException() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => userService.ExecuteBuyTrade(user!, "AAPL", 1));
        }

        [Fact]
        public async Task ExecuteSellTrade_WithInvalidStock_ShouldThrowException() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var portfolioReturn = await userManager.LoadUserPortfolio(user!);
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => userService.ExecuteSellTrade(user!, "FAKESTOCK", 1));
        }

        [Fact]
        public async Task ExecuteSellTrade_WithoutPortfolioLoaded_ShouldThrowException() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => userService.ExecuteSellTrade(user!, "AAPL", 1));
        }

        [Fact]
        public async Task ExecuteBuyTrade_ThenExecuteSellTrade_ShouldLogBothTrades() {
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var userManager = new UserManager(new DatabaseConnection(config));
            var loginManager = new LoginManager(new DatabaseConnection(config));
            await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");
            var portfolioReturn = await userManager.LoadUserPortfolio(user!);
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 200.00m;
            var userService = new UserService(stocks, userManager);

            // Act
            var returnValue = await userService.ExecuteBuyTrade(user!, "AAPL", 1);
            var returnValue2 = await userService.ExecuteSellTrade(user!, "AAPL", 1);

            // Assert
            Assert.True(returnValue);
            Assert.True(returnValue2);
            Assert.Equal(2, user!.Trades!.Count);
            Assert.Empty(user.Portfolio!.Positions!);
        }
    }
}
