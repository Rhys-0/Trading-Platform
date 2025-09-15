using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Models;

namespace TradingApp.Tests.Models {
    public class StocksTests {
        [Fact]
        public void Constructor_ShouldCreateStockList() {
            var stocks = new Stocks();

            Assert.NotNull(stocks.StockList);
            Assert.True(stocks.StockList.Count > 0);
            Assert.Contains("AAPL", stocks.StockList.Keys);
            Assert.Equal("Apple Inc.", stocks.StockList["AAPL"].Name);
        }
    }
}
