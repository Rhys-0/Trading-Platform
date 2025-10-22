using System.Collections.Concurrent;

namespace TradingApp.Models
{
    internal class Stocks {
        public ConcurrentDictionary<string, Stock> StockPrices { get; } = new() {
            ["AAPL"] = new Stock { Symbol = "AAPL", Name = "Apple Inc.", Price = 0 },
            ["GOOGL"] = new Stock { Symbol = "GOOGL", Name = "Alphabet Inc.", Price = 0 },
            ["MSFT"] = new Stock { Symbol = "MSFT", Name = "Microsoft Corporation", Price = 0 },
            ["AMZN"] = new Stock { Symbol = "AMZN", Name = "Amazon.com Inc.", Price = 0 },
            ["TSLA"] = new Stock { Symbol = "TSLA", Name = "Tesla Inc.", Price = 0 }
        };

        public ConcurrentDictionary<string, Stock> StockList => StockPrices;

        internal event Action<string, decimal>? PriceUpdated;

        internal void SetPrice(string symbol, decimal price) {
            StockPrices.AddOrUpdate(
                symbol,
                s => new Stock(s, s, price),
                (s, existing) =>
                {
                    existing.Price = price;
                    return existing;
                });

            PriceUpdated?.Invoke(symbol, price);
        }
    }
}