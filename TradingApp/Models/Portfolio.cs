

namespace TradingApp.Models {
    public class Portfolio {
        public long PortfolioId { get; }
        public decimal Value { get; set; }
        public decimal NetProfit { get; set; }
        public decimal PercentageReturn { get; set; }
        public Dictionary<string, Position>? Positions { get; set; }

        internal Portfolio(long portfolioId, decimal value, decimal netProfit, decimal percentageReturn) {
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
        /// <remarks>Does not update portfolio value, that is handled by the portfolio update service.</remarks>
        /// <exception cref="ArgumentException">Thrown when there is no position for the given stock symbol</exception>
        public void RemoveStocks(string stockSymbol, int quantity) {
            if(Positions == null) {
                throw new ArgumentException("No positions found.");
            }

            bool returnValue = Positions.TryGetValue(stockSymbol, out Position? position);

            if (position == null || !returnValue) {
                throw new ArgumentException("No position found for the given stock symbol.");
            }

            position.RemoveStocks(quantity);
        }

        /// <summary>
        /// Add a given number of stocks of the given stock symbol to the position.
        /// </summary>
        /// <param name="stockSymbol"></param>
        /// <param name="quantity"></param>
        /// <param name="pricePerStock"></param>
        /// <remarks>Does not update portfolio value, that is handled by the portfolio update service.</remarks>
        public void AddStocks(string stockSymbol, int quantity, decimal pricePerStock) {
            if(Positions == null) {
                Positions = new Dictionary<string, Position>();
            }

            bool returnValue = Positions.TryGetValue(stockSymbol, out Position? position);

            if (position == null || !returnValue) {
                position = new Position(-1, stockSymbol, 0); // PositionId will be set when saved to DB
                Positions[stockSymbol] = position;
            }

            position.AddStocks(quantity, pricePerStock);
        }
    }
}
