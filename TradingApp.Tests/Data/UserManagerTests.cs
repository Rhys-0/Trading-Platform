using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Models;

/* ----------------------------------------------------------------------------
 *  All tests in this file require docker to be installed and running to work!
 * ---------------------------------------------------------------------------- */

namespace TradingApp.Tests.Data {
    public class UserManagerTests : IClassFixture<MockDatabase> {
        private readonly MockDatabase _db;
        public UserManagerTests(MockDatabase db) => _db = db;

        [Fact]
        public async Task GetUser_WithValidParams_ShouldGetUser() {
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

            var userManager = new UserManager(new DatabaseConnection(config));


            // Act
            var user = await userManager.GetUser("testuser");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user!.Username);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("User", user.LastName);
            Assert.Equal(100.00m, user.StartingCashBalance);
            Assert.Equal(100.00m, user.CurrentCashBalance);
        }

        [Fact]
        public async Task GetUser_WithInvalidCapitalisation_ShouldReturnNull() {
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

            var userManager = new UserManager(new DatabaseConnection(config));


            // Act
            var user = await userManager.GetUser("testUser");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetUser_WithNonExistantUser_ShouldReturnNull() {
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

            var userManager = new UserManager(new DatabaseConnection(config));


            // Act
            var user = await userManager.GetUser("fakeUser");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task UpdateUser_WithValidParams_ShouldUpdateUser() {
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

            var userManager = new UserManager(new DatabaseConnection(config));

            var updatedUser = new User(1, "testuser", "test@email.com", "Test", "User", 100.00m, 500.00m); // updating balance to 500.00

            // Act
            var returnValue = await userManager.UpdateUser(updatedUser);

            var newUser = await _db.QueryFirstOrDefaultAsync<User>(
                "SELECT user_id AS id, " +
                "username, " +
                "email, " +
                "first_name AS firstName, " +
                "last_name AS lastName, " +
                "starting_cash_balance AS startingCashBalance, " +
                "current_cash_balance AS currentCashBalance " +
                "FROM users " +
                "WHERE username = @Username",
                new { Username = "testuser" });

            Assert.True(returnValue);
            Assert.NotNull(newUser);
            Assert.Equal(500.00m, newUser.CurrentCashBalance);
        }

        [Fact]
        public async Task UpdateUser_ThenGetUser_ShouldReturnUpdatedUser() {
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

            var userManager = new UserManager(new DatabaseConnection(config));

            var updatedUser = new User(1, "testuser", "test@email.com", "Test", "User", 100.00m, 500.00m); // updating balance to 500.00

            // Act
            var returnValue = await userManager.UpdateUser(updatedUser);
            var newUser = await userManager.GetUser("testuser");

            Assert.True(returnValue);
            Assert.NotNull(newUser);
            Assert.Equal(500.00m, newUser.CurrentCashBalance);
        }

        [Fact]
        public async Task UpdateUser_WithInvalidUser_ShouldReturnFalse() {
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

            var userManager = new UserManager(new DatabaseConnection(config));

            var updatedUser = new User(0, "fakeuser", "test@email.com", "Test", "User", 100.00m, 500.00m); // updating balance to 500.00

            // Act
            var returnValue = await userManager.UpdateUser(updatedUser);

            // Assert
            Assert.False(returnValue);
        }

        [Fact]
        public async Task LoadUserPortfolio_WithValidParamsAndEmptyPortfolio_ShouldLoadPortfolio() {
            // Arrange
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));

            // Act
            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User"); // Adds a user with a portfolio row
            var user = await userManager.GetUser("testuser");
            var portfolioResponse = await userManager.LoadUserPortfolio(user!);

            // Assert
            Assert.True(response);
            Assert.True(portfolioResponse);
            Assert.NotNull(user);
            Assert.NotNull(user.Portfolio);
            Assert.NotNull(user.Portfolio!.Positions);
        }

        [Fact]
        public async Task LoadUserPortfolio_WithValidParams_ShouldLoadPortfolio() {
            // Arrange
            await _db.ResetDatabaseAsync();

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));

            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User"); // Adds a user with a portfolio row
            var user = await userManager.GetUser("testuser");
            var portfolioID = await _db.QueryFirstOrDefaultAsync<long>(@"
                UPDATE portfolio SET 
                value = 11000.00,
                net_profit = 1000.00,
                percentage_return = 0.10 
                WHERE user_id = @UserId
                RETURNING portfolio_id;",
                new { UserId = user!.Id });

            var tradeID = await _db.QueryFirstOrDefaultAsync<long>(@"
                INSERT INTO trade 
                (user_id, trade_type, stock_symbol, quantity, price) 
                VALUES (@UserId, 'BUY', 'AAPL', 2, 500.00) 
                RETURNING trade_id",
                new { UserId = user!.Id });

            var positionID = await _db.QueryFirstOrDefaultAsync<long>(@"
                INSERT INTO position 
                (portfolio_id, stock_symbol, total_quantity) 
                VALUES (@PortfolioID, 'AAPL', 2)
                RETURNING position_id",
                new { PortfolioID = portfolioID });

            await _db.ExecuteAsync(@"
                INSERT INTO purchase_lot 
                (position_id, trade_id, quantity, purchase_price) 
                VALUES (@PositionID, @TradeID, 2, 1000.00)",
                new { PositionID = positionID, TradeID = tradeID });

            // Act
            var portfolioResponse = await userManager.LoadUserPortfolio(user!);

            // Assert
            Assert.True(response);
            Assert.True(portfolioResponse);
            Assert.NotNull(user);
            Assert.NotNull(user.Portfolio);
            Assert.NotNull(user.Portfolio.Positions);
            Assert.Single(user.Portfolio.Positions);
            Assert.True(user.Portfolio.Positions.ContainsKey("AAPL"));
            Assert.Equal(2, user.Portfolio.Positions["AAPL"].TotalQuantity);
            Assert.Single(user.Portfolio.Positions["AAPL"].PurchaseLots!);
            Assert.Equal(2, user.Portfolio.Positions["AAPL"].PurchaseLots![0].Quantity);
            Assert.Equal(1000.00m, user.Portfolio.Positions["AAPL"].PurchaseLots![0].PurchasePrice);
        }

        [Fact]
        public async Task LoadUserTrades_WithValidParams_ShouldLoadTrades() {
            // Arrange
            await _db.ResetDatabaseAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();
            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));

            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User"); // Adds a user with a portfolio row
            var user = await userManager.GetUser("testuser");

            await _db.ExecuteAsync(@"
                INSERT INTO trade 
                (user_id, trade_type, stock_symbol, quantity, price) 
                VALUES (@UserId, 'BUY', 'AAPL', 2, 500.00)",
                new { UserId = user!.Id });

            // Act
            await userManager.LoadUserTrades(user!);

            // Assert
            Assert.True(response);
            Assert.NotNull(user);
            Assert.NotNull(user.Trades);
            Assert.Single(user.Trades);
            Assert.Equal("BUY", user.Trades[0].TradeType);
            Assert.Equal("AAPL", user.Trades[0].StockSymbol);
            Assert.Equal(2, user.Trades[0].Quantity);
        }

        [Fact]
        public async Task LoadUserTrades_WithValidParamsAndNoTrades_ShouldLoadEmptyList() {
            // Arrange
            await _db.ResetDatabaseAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();
            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));

            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User"); // Adds a user with a portfolio row
            var user = await userManager.GetUser("testuser");

            // Act
            await userManager.LoadUserTrades(user!);

            // Assert
            Assert.True(response);
            Assert.NotNull(user);
            Assert.NotNull(user.Trades);
            Assert.Empty(user.Trades);
        }

        [Fact]
        public async Task LogTrade_WithValidParamsAndBuy_ShouldLogTrade() {
            // Arrange
            await _db.ResetDatabaseAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));
            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");

            // Act
            var tradeId = await userManager.LogTrade(user!, "AAPL", 10, 200.00m, "BUY");
            var trade = await _db.QueryFirstOrDefaultAsync<Trade>(@"
                SELECT trade_id AS TradeId,
                       trade_type AS TradeType,
                       stock_symbol AS StockSymbol,
                       quantity, 
                       price, 
                       time 
                FROM trade
                WHERE user_id = @UserId",
                new { UserId = user!.Id });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(trade);
            Assert.True(response);
            Assert.NotNull(user.Trades);
            Assert.Single(user.Trades);
            Assert.Equal("BUY", user.Trades[0].TradeType);
            Assert.Equal("AAPL", user.Trades[0].StockSymbol);
            Assert.Equal(tradeId, trade.TradeId);
            Assert.Equal(10, user.Trades[0].Quantity);
            Assert.Equal(trade.Time, user.Trades[0].Time);
            Assert.Equal(trade.TradeType, user.Trades[0].TradeType);
            Assert.Equal(trade.StockSymbol, user.Trades[0].StockSymbol);
        }

        [Fact]
        public async Task LogTrade_WithValidParamsAndSell_ShouldLogTrade() {
            // Arrange
            await _db.ResetDatabaseAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));
            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");

            // Act
            var tradeId = await userManager.LogTrade(user!, "AAPL", 10, 200.00m, "SELL");
            var trade = await _db.QueryFirstOrDefaultAsync<Trade>(@"
                SELECT trade_id AS TradeId,
                       trade_type AS TradeType,
                       stock_symbol AS StockSymbol,
                       quantity, 
                       price, 
                       time 
                FROM trade
                WHERE user_id = @UserId",
                new { UserId = user!.Id });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(trade);
            Assert.True(response);
            Assert.NotNull(user.Trades);
            Assert.Single(user.Trades);
            Assert.Equal("SELL", user.Trades[0].TradeType);
            Assert.Equal("AAPL", user.Trades[0].StockSymbol);
            Assert.Equal(tradeId, trade.TradeId);
            Assert.Equal(10, user.Trades[0].Quantity);
            Assert.Equal(trade.Time, user.Trades[0].Time);
            Assert.Equal(trade.TradeType, user.Trades[0].TradeType);
            Assert.Equal(trade.StockSymbol, user.Trades[0].StockSymbol);
        }

        [Fact]
        public async Task LogTrade_WithInvalidTradeType_ShouldThrowException() {
            // Arrange
            await _db.ResetDatabaseAsync();
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var loginManager = new LoginManager(new DatabaseConnection(config));
            var userManager = new UserManager(new DatabaseConnection(config));
            var response = await loginManager.AddUser("testuser", "test@email.com", "hashedPass", "Test", "User");
            var user = await userManager.GetUser("testuser");

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => userManager.LogTrade(user!, "AAPL", 10, 200.00m, "INVALID"));
            var trade = await _db.QueryFirstOrDefaultAsync<Trade>(@"
                SELECT trade_id AS TradeId,
                       trade_type AS TradeType,
                       stock_symbol AS StockSymbol,
                       quantity, 
                       price, 
                       time 
                FROM trade
                WHERE user_id = @UserId",
                new { UserId = user!.Id });

            // Assert
            Assert.NotNull(user);
            Assert.Null(trade);
            Assert.True(response);
            Assert.Null(user.Trades);
            Assert.Equal("Invalid trade type. Must be 'BUY' or 'SELL'.", ex.Message);
        }
    }
}
