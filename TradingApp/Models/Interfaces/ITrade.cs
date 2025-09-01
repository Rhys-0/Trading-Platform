namespace TradingApp.Models.Interfaces {
    internal interface ITrade {
        int TradeId { get; }
        string TradeType { get; } // BUY or SELL
        string StockSymbol { get; }
        int Quantity { get; }
        decimal Price { get; }
        DateTime Time { get; }
    }
}
