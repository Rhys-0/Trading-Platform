namespace TradingApp.Models {
    public class Trade {
        public long TradeId { get; set; }
        public long UserId { get; set; }
        public string TradeType { get; set; } = "";
        public string StockSymbol { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime Time { get; set; }

        public Trade() { }

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
