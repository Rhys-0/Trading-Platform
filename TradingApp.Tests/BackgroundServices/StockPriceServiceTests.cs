using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingApp.BackgroundServices;
using TradingApp.Data;
using TradingApp.Models;

namespace TradingApp.Tests.BackgroundServices {
    public class StockPriceServiceTests {
        [Fact(Skip = "Integration test requiring API keys")]
        public void Constructor_WithValidParams_ShouldUpdatePrice() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var mock = new Mock<ILogger<StockPriceService>>();
            ILogger<StockPriceService> logger = mock.Object;
            Stocks stocks = new();

            _ = new StockPriceService(logger, config, stocks);

            Assert.NotEqual(0m, stocks.StockList["AAPL"].Price);
        }
    }
}
