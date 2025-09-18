namespace TradingApp.Models {
    internal class Stock {
        internal string Symbol { get; }
        internal string Name { get; }
        internal decimal Price { get; set; }

        internal Stock(string symbol, string name, decimal price) {
            Symbol = symbol;
            Name = name;
            Price = price;
        }
    }
}
