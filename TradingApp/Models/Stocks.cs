using System.Collections.Concurrent;

namespace TradingApp.Models
{
    public class Stocks {
        public ConcurrentDictionary<string, Stock> StockPrices { get; } = new() {
            // NASDAQ mega-cap
            ["AAPL"] = new Stock { Symbol = "AAPL", Name = "Apple Inc.", Price = 0 },
            ["GOOGL"] = new Stock { Symbol = "GOOGL", Name = "Alphabet Inc.", Price = 0 },
            ["MSFT"] = new Stock { Symbol = "MSFT", Name = "Microsoft Corporation", Price = 0 },
            ["AMZN"] = new Stock { Symbol = "AMZN", Name = "Amazon.com Inc.", Price = 0 },
            ["TSLA"] = new Stock { Symbol = "TSLA", Name = "Tesla Inc.", Price = 0 },
            ["NVDA"] = new Stock { Symbol = "NVDA", Name = "NVIDIA Corporation", Price = 0 },
            ["META"] = new Stock { Symbol = "META", Name = "Meta Platforms, Inc.", Price = 0 },
            ["NFLX"] = new Stock { Symbol = "NFLX", Name = "Netflix, Inc.", Price = 0 },
            ["AMD"] = new Stock { Symbol = "AMD", Name = "Advanced Micro Devices, Inc.", Price = 0 },
            ["INTC"] = new Stock { Symbol = "INTC", Name = "Intel Corporation", Price = 0 },
            ["AVGO"] = new Stock { Symbol = "AVGO", Name = "Broadcom Inc.", Price = 0 },

            // NYSE & ETFs
            ["KO"] = new Stock { Symbol = "KO", Name = "Coca-Cola Company", Price = 0 },
            ["DIS"] = new Stock { Symbol = "DIS", Name = "Walt Disney Company", Price = 0 },
            ["JPM"] = new Stock { Symbol = "JPM", Name = "JPMorgan Chase & Co.", Price = 0 },
            ["BAC"] = new Stock { Symbol = "BAC", Name = "Bank of America Corp", Price = 0 },
            ["WMT"] = new Stock { Symbol = "WMT", Name = "Walmart Inc.", Price = 0 },
            ["PEP"] = new Stock { Symbol = "PEP", Name = "PepsiCo, Inc.", Price = 0 },
            ["NKE"] = new Stock { Symbol = "NKE", Name = "NIKE, Inc.", Price = 0 },
            ["IBM"] = new Stock { Symbol = "IBM", Name = "International Business Machines", Price = 0 },
            ["ORCL"] = new Stock { Symbol = "ORCL", Name = "Oracle Corporation", Price = 0 },
            ["TSM"] = new Stock { Symbol = "TSM", Name = "Taiwan Semiconductor Mfg.", Price = 0 }
        };

        public ConcurrentDictionary<string, Stock> StockList => StockPrices;

        public event Action<string, decimal>? PriceUpdated;

        public void SetPrice(string symbol, decimal price) {
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
} // test