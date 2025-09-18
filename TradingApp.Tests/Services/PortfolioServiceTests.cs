using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Models;
using TradingApp.Services;
using Xunit;

namespace TradingApp.Tests.Services {
    public class PortfolioServiceTests {
        [Fact]
        public void Constructor_WithValidParams_ShouldInitaliseValues() {
            // Arrange
            var stocks = new Stocks();
            // Act
            var portfolioService = new PortfolioService(stocks);
            // Assert
            Assert.NotNull(portfolioService);
        }

        [Fact]
        public void UpdateUserPortfolio_WithValidParams_ShouldUpdatePortfolioValue() {
            // Arrange
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 75.00m;
            stocks.StockList["MSFT"].Price = 30.00m;
            var portfolioService = new PortfolioService(stocks);
            var user = new User(1, "testuser", "test@email.com", "Test", "User", 100.00m, 0.00m);
            user.Portfolio = new Portfolio(1, 0.00m, 0.00m, 0.00m);
            user.Portfolio.AddStocks("AAPL", 1, 50.00m);
            user.Portfolio.AddStocks("MSFT", 2, 25.00m);

            // Act
            portfolioService.UpdateUserPortfolio(user);
            // Assert
            Assert.Equal(135.00m, user.Portfolio.Value);
            Assert.Equal(35.00m, user.Portfolio.NetProfit);
            Assert.Equal(0.35m, user.Portfolio.PercentageReturn);
        }

        [Fact]
        public void UpdateUserPortfolio_WithoutPortfolioLoaded_ShouldThrowException() {
            // Arrange
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 75.00m;
            stocks.StockList["MSFT"].Price = 30.00m;
            var portfolioService = new PortfolioService(stocks);
            var user = new User(1, "testuser", "test@email.com", "Test", "User", 100.00m, 0.00m);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => portfolioService.UpdateUserPortfolio(user));
        }

        [Fact]
        public void UpdateUserPortfolio_WithInvalidStockSymbol_ShouldThrowException() {
            // Arrange
            var stocks = new Stocks();
            stocks.StockList["AAPL"].Price = 75.00m;
            stocks.StockList["MSFT"].Price = 30.00m;
            var portfolioService = new PortfolioService(stocks);
            var user = new User(1, "testuser", "test@email.com", "Test", "User", 100.00m, 0.00m);
            user.Portfolio = new Portfolio(1, 0.00m, 0.00m, 0.00m);
            user.Portfolio.AddStocks("FAKE", 1, 50.00m);
            user.Portfolio.AddStocks("FAKE2", 2, 25.00m);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => portfolioService.UpdateUserPortfolio(user));
        }
    }
}
