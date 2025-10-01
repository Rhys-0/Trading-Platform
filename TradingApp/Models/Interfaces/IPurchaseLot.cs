namespace TradingApp.Models.Interfaces {
    public interface IPurchaseLot {
        int PurchaseLotId { get; }
        int Quantity { get; set; }
        decimal PurchasePrice { get; }
        DateTime PurchaseDate { get; }
    }
}
