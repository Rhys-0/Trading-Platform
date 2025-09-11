using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal sealed class Position : IPosition {
        public int PositionId { get; }
        public string StockSymbol { get; }
        public int TotalQuantity { get; set; }
        public List<IPurchaseLot>? PurchaseLots {  get; set; }

        internal Position(int positionId, string stockSymbol, int totalQuantity) { 
            PositionId = positionId;
            StockSymbol = stockSymbol;
            TotalQuantity = totalQuantity;
        }

        /// <summary>
        /// Calculates which stocks to sell based on the quantity requested using a FIFO method.
        /// </summary>
        /// <param name="quantity">The quantity of stocks to sell.</param>
        /// <returns></returns>
        /// <remarks>This method does not update the database!</remarks>
        public List<IPurchaseLot>? SellStocks(int quantity) {
            throw new NotImplementedException();
        }
    }
}
