namespace TradingApp.Models
{
    public class PurchaseLot
    {
        public long PurchaseLotId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; }
        public DateTime PurchaseDate { get; }

        // Parameterless constructor for Dapper
        public PurchaseLot()
        {
            PurchasePrice = 0;
            PurchaseDate = DateTime.UtcNow;
        }

        internal PurchaseLot(long purchaseLotId, decimal quantity, decimal purchasePrice, DateTime purchaseDate)
        {
            PurchaseLotId = purchaseLotId;
            Quantity = quantity;
            PurchasePrice = purchasePrice;
            PurchaseDate = purchaseDate;
        }
    }
}