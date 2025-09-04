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

        public List<IPurchaseLot>? SellStocks(int quantity) {
            throw new NotImplementedException();
        }

        public void LoadPurchaseLots() {
            throw new NotImplementedException();
        }
    }
}
