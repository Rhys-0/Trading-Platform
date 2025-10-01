using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp.Tests.Models {
    public class TradeTests {
        [Fact]
        public static void Constructor_WithValidParams_ShouldReturnValidTrade() {
            var tradeId = 1;
            var tradeType = "BUY";
            var stockSymbol = "AAPL";
            var quantity = 10;
            var price = 1500.25m;
            var time = DateTime.UtcNow;

            var trade = new TradingApp.Models.Trade(tradeId, tradeType, stockSymbol, quantity, price, time);

            Assert.Equal(tradeId, trade.TradeId);
            Assert.Equal(tradeType, trade.TradeType);
            Assert.Equal(stockSymbol, trade.StockSymbol);
            Assert.Equal(quantity, trade.Quantity);
            Assert.Equal(price, trade.Price);
            Assert.Equal(time, trade.Time);
        }
    }
}
