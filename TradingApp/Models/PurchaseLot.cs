using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal sealed class PurchaseLot : IPurchaseLot {
        public int PurchaseLotId { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; }

        public DateTime PurchaseDate { get; }

        internal PurchaseLot(int purchaseLotId, int quantity, decimal purchasePrice, DateTime purchaseDate) {
            PurchaseLotId = purchaseLotId;
            Quantity = quantity;
            PurchasePrice = purchasePrice;
            PurchaseDate = purchaseDate;
        }
    }
}
