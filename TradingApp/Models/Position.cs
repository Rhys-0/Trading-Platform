namespace TradingApp.Models
{
    public class Position
    {
        public long PositionId { get; set; }
        public string StockSymbol { get; }
        public int TotalQuantity { get; set; }
        public List<PurchaseLot>? PurchaseLots { get; set; }

        // Parameterless constructor for Dapper
        public Position()
        {
            StockSymbol = string.Empty;
        }

        internal Position(long positionId, string stockSymbol, int totalQuantity)
        {
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
        public void RemoveStocks(int quantity)
        {
            if (PurchaseLots == null || PurchaseLots.Count == 0 || quantity <= 0 || quantity > TotalQuantity)
            {
                throw new ArgumentException("Invalid quantity to sell or no purchase lots available.");
            }

            // Sort purchase lots by purchase date ascending (oldest first)
            List<PurchaseLot> sortedAscending = PurchaseLots.OrderBy(lot => lot.PurchaseDate).ToList();

            int remainingToSell = quantity;
            foreach (PurchaseLot lot in sortedAscending)
            {
                if (remainingToSell <= 0) break;

                // Cast lot.Quantity to int for comparison
                int lotQuantityInt = (int)lot.Quantity;

                if (lotQuantityInt <= remainingToSell)
                {
                    // Sell the entire lot
                    remainingToSell -= lotQuantityInt;
                    lot.Quantity = 0;
                }
                else
                {
                    // Sell part of the lot
                    lot.Quantity -= remainingToSell;
                    remainingToSell = 0;
                }
            }

            // Remove lots that are fully sold using LINQ
            PurchaseLots = PurchaseLots.Where(lot => lot.Quantity > 0).ToList();
            TotalQuantity -= quantity;
        }

        /// <summary>
        /// Add stocks to the position.
        /// </summary>
        /// <param name="quantity">The quantity of shares to add</param>
        /// <param name="pricePerStock">The price per share</param>
        /// <exception cref="ArgumentException">Thrown if quantity or price is invalid</exception>
        public void AddStocks(int quantity, decimal pricePerStock)
        {
            if (quantity <= 0 || pricePerStock <= 0)
            {
                throw new ArgumentException("Invalid quantity or price per stock.");
            }
            // If purchase lot is null create an empty list
            PurchaseLots ??= [];

            // Create a new purchase lot for the added stocks
            // ID will be set when saved to DB
            PurchaseLot newLot = new PurchaseLot(-1, quantity, pricePerStock, DateTime.UtcNow);

            PurchaseLots.Add(newLot);
            TotalQuantity += quantity;
        }
    }
}