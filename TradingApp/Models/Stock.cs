using System.Diagnostics.CodeAnalysis;

namespace TradingApp.Models {
    public class Stock {
        public required string Symbol { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }

        public Stock() { }

        [SetsRequiredMembers]
        public Stock(string symbol, string name, decimal price) {
            Symbol = symbol;
            Name = name;
            Price = price;
        }
    }
}
