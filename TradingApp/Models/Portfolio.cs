using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal class Portfolio : IPortfolio {
        public int PortfolioId { get; }
        public decimal Value { get; set; }
        public decimal NetProfit { get; set; }
        public decimal PercentageReturn { get; set; }
        public List<IPosition>? Positions { get; set; }

        internal Portfolio(int portfolioId, int value, decimal netProfit, decimal percentageReturn) {
            PortfolioId = portfolioId;
            Value = value;
            NetProfit = netProfit;
            PercentageReturn = percentageReturn;
        }

        public void LoadPositions() {
            throw new NotImplementedException();
        }
    }
}
