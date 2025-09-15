namespace TradingApp.Models {
    internal class Stocks {
        internal Dictionary<string, Stock> StockList { get; }

        public Stocks() {
            StockList = new Dictionary<string, Stock> {
                { "AAPL", new Stock("AAPL", "Apple Inc.", 0.00m) },
                { "GOOGL", new Stock("GOOGL", "Alphabet Inc.", 0.00m) },
                { "MSFT", new Stock("MSFT", "Microsoft Corporation", 0.00m) },
                { "AMZN", new Stock("AMZN", "Amazon.com, Inc.", 0.00m) },
                { "TSLA", new Stock("TSLA", "Tesla, Inc.", 0.00m) }
            };
        }
    }
}
