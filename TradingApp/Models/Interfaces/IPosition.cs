namespace TradingApp.Models.Interfaces {
    public interface IPosition {
        int PositionId { get; }
        string StockSymbol { get; }
        int TotalQuantity { get; set; }
        List<IPurchaseLot>? PurchaseLots { get; set; }

        internal List<IPurchaseLot>? SellStocks(int quantity);
        internal void LoadPurchaseLots();
    }
}
