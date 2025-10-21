using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TradingApp.Models;
using Xunit;

namespace TradingApp.Tests.Models
{
    public class HoldingsManagerTests
    {
        private static PurchaseLot CreatePurchaseLot(long id, int qty, decimal price, DateTime date)
        {
            var ctor = typeof(PurchaseLot)
                .GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(long), typeof(int), typeof(decimal), typeof(DateTime) },
                    null)!;

            return (PurchaseLot)ctor.Invoke(new object[] { id, qty, price, date });
        }

        private static Position CreatePosition(long id, string symbol, int totalQty)
        {
            var ctor = typeof(Position)
                .GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(long), typeof(string), typeof(int) },
                    null)!;

            return (Position)ctor.Invoke(new object[] { id, symbol, totalQty });
        }

        [Fact]
        public void LoadHoldings_ShouldCalculateTotalsAndWeights()
        {
            var manager = new HoldingsManager();

            var posAapl = CreatePosition(1, "AAPL", 10);
            posAapl.PurchaseLots = new List<PurchaseLot>
            {
                CreatePurchaseLot(1, 5, 100m, DateTime.UtcNow.AddDays(-5)),
                CreatePurchaseLot(2, 5, 120m, DateTime.UtcNow.AddDays(-2))
            };

            var posMsft = CreatePosition(2, "MSFT", 5);
            posMsft.PurchaseLots = new List<PurchaseLot>
            {
                CreatePurchaseLot(3, 5, 200m, DateTime.UtcNow.AddDays(-1))
            };

            var positions = new Dictionary<string, Position>
            {
                ["AAPL"] = posAapl,
                ["MSFT"] = posMsft
            };

            var stocks = new Dictionary<string, Stock>
            {
                ["AAPL"] = new Stock("AAPL", "Apple Inc.", 150m),
                ["MSFT"] = new Stock("MSFT", "Microsoft", 250m)
            };

            manager.LoadHoldings(positions, stocks);

            Assert.Equal(2, manager.Holdings.Count);
            Assert.True(manager.TotalValue > 0);
            Assert.Equal("Apple Inc.", manager.Holdings.First(h => h.Ticker == "AAPL").Name);
            Assert.True(manager.Holdings.All(h => h.PortfolioWeight > 0));
            Assert.True(manager.FlatChange > 0);
            Assert.True(manager.PercentageChange > 0);
        }

        [Fact]
        public void LoadHoldings_ShouldHandleMissingStockGracefully()
        {
            var manager = new HoldingsManager();

            var fakePos = CreatePosition(3, "FAKE", 10);
            fakePos.PurchaseLots = new List<PurchaseLot>
            {
                CreatePurchaseLot(4, 10, 50m, DateTime.UtcNow)
            };

            var positions = new Dictionary<string, Position>
            {
                ["FAKE"] = fakePos
            };

            var stocks = new Dictionary<string, Stock>();

            manager.LoadHoldings(positions, stocks);

            Assert.Empty(manager.Holdings);
        }

        [Fact]
        public void LoadHoldings_ShouldHandleNullInputsGracefully()
        {
            // Arrange
            var manager = new HoldingsManager();

            // Act
            manager.LoadHoldings(null!, null!);

            // Assert
            Assert.Empty(manager.Holdings);
            Assert.Equal(0, manager.TotalValue);
        }
    }
}
