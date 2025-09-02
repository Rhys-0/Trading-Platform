using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal class Trade : ITrade {
        public int TradeId { get; }
        public string TradeType { get; }
        public string StockSymbol { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        public DateTime Time { get; }

        internal Trade(int tradeId, string tradeType, string stockSymbol, int quantity, decimal price, DateTime time) {
            TradeId = tradeId;
            TradeType = tradeType;
            StockSymbol = stockSymbol;
            Quantity = quantity;
            Price = price;
            Time = time;
        }
    }
}
