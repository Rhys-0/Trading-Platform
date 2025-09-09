using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void SellStocks_WithValidQuantity_ShouldReturnUpdatedPurchaseLots() {
            // TODO: Implement test
            Assert.True(true);
        }

        [Fact]
        public static void SellStocks_WithInvalidQuantity_ShouldReturnNull() {
            // TODO: Implement test
            Assert.True(true);
        }

        [Fact]
        public static void LoadPurchaseLots_ShouldLoadPurchaseLots() {
            // TODO: Implement test
            Assert.True(true);
        }
    }
}
