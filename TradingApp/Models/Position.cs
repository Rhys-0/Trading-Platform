using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal sealed class Position : IPosition {
        public int PositionId { get; set; }
        public string StockSymbol { get; }
        public int TotalQuantity { get; set; }
        public List<IPurchaseLot>? PurchaseLots { get; set; }

        internal Position(int positionId, string stockSymbol, int totalQuantity) {
            PositionId = positionId;
            StockSymbol = stockSymbol;
            TotalQuantity = totalQuantity;
        }

        /// <summary>
        /// Calculates which stocks to sell based on the quantity requested using a FIFO method 
        /// then updates the purchase lots and total quantity accordingly.
        /// </summary>
        /// <param name="quantity">The quantity of stocks to sell.</param>
        /// <remarks>This method does not update the database!</remarks>
        /// <exception cref="ArgumentException">Thrown if the quantity is invalid</exception>
        public void RemoveStocks(int quantity) {
            if (PurchaseLots == null || PurchaseLots.Count == 0 || quantity <= 0 || quantity > TotalQuantity) {
                throw new ArgumentException("Invalid quantity to sell or no purchase lots available.");
            }

            // Sort purchase lots by purchase date ascending (oldest first)
            List<IPurchaseLot> sortedAscending = PurchaseLots.OrderBy(lot => lot.PurchaseDate).ToList();

            int remainingToSell = quantity;
            foreach (IPurchaseLot lot in sortedAscending) {
                if (remainingToSell <= 0) break;
                if (lot.Quantity <= remainingToSell) {
                    // Sell the entire lot
                    remainingToSell -= lot.Quantity;
                    lot.Quantity = 0;
                } else {
                    // Sell part of the lot
                    lot.Quantity -= remainingToSell;
                    remainingToSell = 0;
                }
            }

            // Remove lots that are fully sold using LINQ
            PurchaseLots = PurchaseLots.Where(lot => lot.Quantity > 0).ToList();
        }

        public void AddStocks(int quantity, decimal pricePerStock) {
            if (quantity <= 0 || pricePerStock <= 0) {
                throw new ArgumentException("Invalid quantity or price per stock.");
            }
            // If purchase lot is null create an empty list
            PurchaseLots ??= [];

            // Create a new purchase lot for the added stocks
            // ID will be set when saved to DB
            IPurchaseLot newLot = new PurchaseLot(-1, quantity, pricePerStock, DateTime.UtcNow);

            PurchaseLots.Add(newLot);
            TotalQuantity += quantity;
        }
    }
}
