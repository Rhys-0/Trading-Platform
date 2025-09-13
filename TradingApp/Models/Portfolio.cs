using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal sealed class Portfolio : IPortfolio {
        public int PortfolioId { get; }
        public decimal Value { get; set; }
        public decimal NetProfit { get; set; }
        public decimal PercentageReturn { get; set; }
        public Dictionary<string, IPosition>? Positions { get; set; }

        internal Portfolio(int portfolioId, decimal value, decimal netProfit, decimal percentageReturn) {
            PortfolioId = portfolioId;
            Value = value;
            NetProfit = netProfit;
            PercentageReturn = percentageReturn;
        }

        /// <summary>
        /// Sell a given number of stocks from the position associated with the given stock symbol.
        /// </summary>
        /// <param name="stockSymbol"></param>
        /// <param name="quantity"></param>
        /// <exception cref="ArgumentException">Thrown when there is no position for the given stock symbol</exception>
        public void RemoveStocks(string stockSymbol, int quantity) {
            if(Positions == null) {
                throw new ArgumentException("No position found for the given stock symbol.");
            }

            bool returnValue = Positions.TryGetValue(stockSymbol, out IPosition? position);

            if (position == null || !returnValue) {
                throw new ArgumentException("No position found for the given stock symbol.");
            }

            position.RemoveStocks(quantity);
        }

        public void AddStocks(string stockSymbol, int quantity, decimal pricePerStock) {
            if(Positions == null) {
                Positions = new Dictionary<string, IPosition>();
            }

            bool returnValue = Positions.TryGetValue(stockSymbol, out IPosition? position);

            if (position == null || !returnValue) {
                position = new Position(-1, stockSymbol, 0); // PositionId will be set when saved to DB
            }

            position.AddStocks(quantity, pricePerStock);
        }
    }
}
