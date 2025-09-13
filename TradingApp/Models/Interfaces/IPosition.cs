namespace TradingApp.Models.Interfaces {
    internal interface IPosition {
        int PositionId { get; set; }
        string StockSymbol { get; }
        int TotalQuantity { get; set; }
        List<IPurchaseLot>? PurchaseLots { get; set; }

        internal void RemoveStocks(int quantity);
        internal void AddStocks(int quantity, decimal pricePerStock);
    }
}
