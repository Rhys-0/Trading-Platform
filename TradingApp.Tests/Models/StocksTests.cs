using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.Models;
using Xunit;

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
        [Fact]
        public void SetPrice_UpdatesExisting_AndRaisesEvent() {
            var stocks = new Stocks();
            string? seenSym = null; decimal seenPrice = 0;

            stocks.PriceUpdated += (s, p) => { seenSym = s; seenPrice = p; };

            stocks.SetPrice("AAPL", 123.45m);

            Assert.Equal("AAPL", seenSym);
            Assert.Equal(123.45m, seenPrice);
            Assert.Equal(123.45m, stocks.StockList["AAPL"].Price);
        }
        [Fact]
        public void SetPrice_AddsNewSymbol_AndRaisesEvent() {
            var stocks = new Stocks();
            _ = stocks.StockList.Remove("TSLA", out _);

            string? seenSym = null; decimal seenPrice = 0;
            stocks.PriceUpdated += (s, p) => { seenSym = s; seenPrice = p; };

            stocks.SetPrice("TSLA", 456.78m);

            Assert.True(stocks.StockList.ContainsKey("TSLA"));
            Assert.Equal(456.78m, stocks.StockList["TSLA"].Price);
            Assert.Equal("TSLA", seenSym);
            Assert.Equal(456.78m, seenPrice);
        }
    }
}
