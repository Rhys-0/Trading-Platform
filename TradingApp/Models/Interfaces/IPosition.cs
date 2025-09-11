namespace TradingApp.Models.Interfaces {
    internal interface IPosition {
        int PositionId { get; }
        string StockSymbol { get; }
        int TotalQuantity { get; set; }
        List<IPurchaseLot>? PurchaseLots { get; set; }

        internal List<IPurchaseLot>? SellStocks(int quantity);
    }
}
