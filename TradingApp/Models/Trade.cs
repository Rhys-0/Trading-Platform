

namespace TradingApp.Models {
    public class Trade {
        public long TradeId { get; }
        public string TradeType { get; }
        public string StockSymbol { get; }
        public int Quantity { get; }
        public decimal Price { get; }
        public DateTime Time { get; }

        internal Trade(long tradeId, string tradeType, string stockSymbol, int quantity, decimal price, DateTime time) {
            TradeId = tradeId;
            TradeType = tradeType;
            StockSymbol = stockSymbol;
            Quantity = quantity;
            Price = price;
            Time = time;
        }
    }
}
