using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp.Tests.Models {
    public class PortfolioTests {
        [Fact]
        public static void Constructor_WithValidParams_ShouldReturnValidPortfolio() {
            var portfolioId = 1;
            var value = 120_220.20m;
            var netProfit = 20_220.20m;
            var percentageReturn = 20.20m;

            var portfolio = new TradingApp.Models.Portfolio(portfolioId, value, netProfit, percentageReturn);

            Assert.Equal(portfolioId, portfolio.PortfolioId);
            Assert.Equal(value, portfolio.Value);
            Assert.Equal(netProfit, portfolio.NetProfit);
            Assert.Equal(percentageReturn, portfolio.PercentageReturn);
            Assert.Null(portfolio.Positions);
        }

        [Fact]
        public static void LoadPositions_ShouldLoadPositions() {
            // TODO: Implement test
            Assert.True(true);
        }
    }
}
