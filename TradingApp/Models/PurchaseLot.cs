

namespace TradingApp.Models {
    public class PurchaseLot {
        public long PurchaseLotId { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; }

        public DateTime PurchaseDate { get; }

        internal PurchaseLot(long purchaseLotId, int quantity, decimal purchasePrice, DateTime purchaseDate) {
            PurchaseLotId = purchaseLotId;
            Quantity = quantity;
            PurchasePrice = purchasePrice;
            PurchaseDate = purchaseDate;
        }
    }
}
