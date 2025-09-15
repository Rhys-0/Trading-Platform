using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Models;


namespace TradingApp.Tests.Models {
    public class PortfolioTests {
        [Fact]
        public static void Constructor_WithValidParams_ShouldReturnValidPortfolio() {
            var portfolioId = 1;
            var value = 120_220.20m;
            var netProfit = 20_220.20m;
            var percentageReturn = 20.20m;

            var portfolio = new Portfolio(portfolioId, value, netProfit, percentageReturn);

            Assert.Equal(portfolioId, portfolio.PortfolioId);
            Assert.Equal(value, portfolio.Value);
            Assert.Equal(netProfit, portfolio.NetProfit);
            Assert.Equal(percentageReturn, portfolio.PercentageReturn);
            Assert.Null(portfolio.Positions);
        }

        [Fact]
        public static void RemoveStocks_WithValidParams_ShouldRemoveStocks() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);
            portfolio.Positions = new Dictionary<string, Position>();
            portfolio.Positions["AAPL"] = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            portfolio.Positions["AAPL"].PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            portfolio.RemoveStocks("AAPL", 70);

            // Assert
            Assert.Equal(30, portfolio.Positions["AAPL"].TotalQuantity);
            Assert.Contains("AAPL", portfolio.Positions);
            Assert.Single(portfolio.Positions["AAPL"].PurchaseLots!);
        }

        [Fact]
        public static void RemoveStocks_WithInvalidAmount_ShouldThrowException() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);
            portfolio.Positions = new Dictionary<string, Position>();
            portfolio.Positions["AAPL"] = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            portfolio.Positions["AAPL"].PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => portfolio.RemoveStocks("AAPL", 110));

            // Assert
            Assert.Equal(100, portfolio.Positions["AAPL"].TotalQuantity);
            Assert.Contains("AAPL", portfolio.Positions);
        }

        [Fact]
        public static void RemoveStocks_WithoutAnyPosition_ShouldThrowException() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);

            var currentDate = DateTime.UtcNow;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => portfolio.RemoveStocks("AAPL", 60));
            Assert.Equal("No positions found.", ex.Message);
        }

        [Fact]
        public static void RemoveStocks_WithInvalidStock_ShouldThrowException() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);
            portfolio.Positions = new Dictionary<string, Position>();
            portfolio.Positions["AAPL"] = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            portfolio.Positions["AAPL"].PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => portfolio.RemoveStocks("MSFT", 50));

            // Assert
            Assert.Equal("No position found for the given stock symbol.", ex.Message);
        }

        [Fact]
        public static void AddStocks_WithValidParamsAndExistingPosition_ShouldAddStocks() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);
            portfolio.Positions = new Dictionary<string, Position>();
            portfolio.Positions["AAPL"] = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            portfolio.Positions["AAPL"].PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            portfolio.AddStocks("AAPL", 70, 160.00m);

            // Assert
            Assert.Equal(170, portfolio.Positions["AAPL"].TotalQuantity);
            Assert.Equal(3, portfolio.Positions["AAPL"].PurchaseLots!.Count);
            Assert.Equal(70, portfolio.Positions["AAPL"].PurchaseLots![2].Quantity);
        }

        [Fact]
        public static void AddStocks_WithValidParamsAndNoPosition_ShouldAddStocks() {
            // Arrange
            var portfolio = new Portfolio(1, 120_220.20m, 20_220.20m, 20.20m);

            // Act
            portfolio.AddStocks("AAPL", 70, 160.00m);

            // Assert
            Assert.NotNull(portfolio.Positions);
            Assert.Equal(70, portfolio.Positions["AAPL"].TotalQuantity);
            Assert.Single(portfolio.Positions["AAPL"].PurchaseLots!);
            Assert.Equal(70, portfolio.Positions["AAPL"].PurchaseLots![0].Quantity);
        }
    }
}
