using System.Collections.Generic;

namespace TradingApp.Models {
    internal class Stocks {
        internal Dictionary<string, Stock> StockList { get; }

        // UI can subscribe to this for live updates
        internal event Action<string, decimal>? PriceUpdated;

        private readonly object _lock = new();

        public Stocks() {
            StockList = new Dictionary<string, Stock> {
                { "AAPL",  new Stock("AAPL",  "Apple Inc.",             0.00m) },
                { "GOOGL", new Stock("GOOGL", "Alphabet Inc.",          0.00m) },
                { "MSFT",  new Stock("MSFT",  "Microsoft Corporation",  0.00m) },
                { "AMZN",  new Stock("AMZN",  "Amazon.com, Inc.",       0.00m) },
                { "TSLA",  new Stock("TSLA",  "Tesla, Inc.",            0.00m) }
            };
        }

        // Call this from background service whenever a price changes
        internal void SetPrice(string symbol, decimal price) {
            lock (_lock) {
                if (StockList.TryGetValue(symbol, out var s)) {
                    s.Price = price;
                } else {
                    StockList[symbol] = new Stock(symbol, symbol, price);
                }
            }
            PriceUpdated?.Invoke(symbol, price);
        }
    }
}
