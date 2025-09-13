namespace TradingApp.Models.Interfaces {
    internal interface IPurchaseLot {
        int PurchaseLotId { get; set; }
        int Quantity { get; set; }
        decimal PurchasePrice { get; }
        DateTime PurchaseDate { get; }
    }
}
