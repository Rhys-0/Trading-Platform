using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Models;


namespace TradingApp.Tests.Models {
    public class PositionTests {
        [Fact]
        public static void Constructor_WithValidParams_ShouldReturnValidPosition() {
            var positionId = 1;
            var stockSymbol = "AAPL";
            var quantity = 10;

            var position = new TradingApp.Models.Position(positionId, stockSymbol, quantity);
            Assert.Equal(positionId, position.PositionId);
            Assert.Equal(stockSymbol, position.StockSymbol);
            Assert.Equal(quantity, position.TotalQuantity);
        }

        [Fact]
        public static void RemoveStocks_WithValidParams_ShouldRemoveStocks() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            position.RemoveStocks(70);

            // Assert
            Assert.Equal(30, position.TotalQuantity);
            Assert.Single(position.PurchaseLots!);
            Assert.Equal(2, position.PurchaseLots![0].PurchaseLotId);
            Assert.Equal(30, position.PurchaseLots![0].Quantity);
            Assert.Equal(155.00m, position.PurchaseLots![0].PurchasePrice);
            Assert.Equal(currentDate.AddDays(-5), position.PurchaseLots![0].PurchaseDate);
        }

        [Fact]
        public static void RemoveStocks_WithInvalidAmount_ShouldThrowException() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => position.RemoveStocks(110));

            // Assert
            Assert.Equal(100, position.TotalQuantity);
            Assert.Equal(2, position.PurchaseLots!.Count);
            Assert.Equal(1, position.PurchaseLots![0].PurchaseLotId);
            Assert.Equal(50, position.PurchaseLots![0].Quantity);
            Assert.Equal(150.00m, position.PurchaseLots![0].PurchasePrice);
            Assert.Equal(currentDate.AddDays(-10), position.PurchaseLots![0].PurchaseDate);
        }

        [Fact]
        public static void RemoveStocks_WithNegativeAmount_ShouldThrowException() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => position.RemoveStocks(-50));

            Assert.Equal(100, position.TotalQuantity);
            Assert.Equal(2, position.PurchaseLots!.Count);
            Assert.Equal(1, position.PurchaseLots![0].PurchaseLotId);
            Assert.Equal(50, position.PurchaseLots![0].Quantity);
            Assert.Equal(150.00m, position.PurchaseLots![0].PurchasePrice);
            Assert.Equal(currentDate.AddDays(-10), position.PurchaseLots![0].PurchaseDate);
        }

        [Fact]
        public static void RemoveStocks_WithNoPurchaseLots_ShouldThrowException() {
            // Arrange
            var position = new Position(1, "AAPL", 0);

            var currentDate = DateTime.UtcNow;

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => position.RemoveStocks(50));

            Assert.Equal(0, position.TotalQuantity);
            Assert.Equal("Invalid quantity to sell or no purchase lots available.", ex.Message);
        }

        [Fact]
        public static void AddStocks_WithValidParams_ShouldAddStocks() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            position.AddStocks(70, 160.00m);

            // Assert
            Assert.Equal(170, position.TotalQuantity);
            Assert.Equal(3, position.PurchaseLots!.Count);
            Assert.Equal(70, position.PurchaseLots![2].Quantity);
        }

        [Fact]
        public static void AddStocks_WithValidInvalidQuantity_ShouldThrowException() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            var ex = Assert.Throws<ArgumentException>(() => position.AddStocks(-70, 160.00m));

            // Assert
            Assert.Equal("Invalid quantity or price per stock.", ex.Message);
        }

        [Fact]
        public static void AddStocks_WithValidInvalidPrice_ShouldThrowException() {
            // Arrange
            var position = new Position(1, "AAPL", 100);

            var currentDate = DateTime.UtcNow;
            position.PurchaseLots = new List<PurchaseLot> {
                new PurchaseLot(1, 50, 150.00m, currentDate.AddDays(-10)),
                new PurchaseLot(2, 50, 155.00m, currentDate.AddDays(-5))
            };

            // Act
            var ex = Assert.Throws<ArgumentException>(() => position.AddStocks(70, -160.00m));

            // Assert
            Assert.Equal("Invalid quantity or price per stock.", ex.Message);
        }

        [Fact]
        public static void AddStocks_WithValidParamsAndNoPurchaseLots_ShouldAddStocks() {
            // Arrange
            var position = new Position(1, "AAPL", 0);

            var currentDate = DateTime.UtcNow;

            // Act
            position.AddStocks(70, 160.00m);

            // Assert
            Assert.NotNull(position.PurchaseLots);
            Assert.Equal(70, position.TotalQuantity);
            Assert.Single(position.PurchaseLots);
            Assert.Equal(70, position.PurchaseLots[0].Quantity);
        }
    }
}
