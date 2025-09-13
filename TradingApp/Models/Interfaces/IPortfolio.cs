namespace TradingApp.Models.Interfaces {
    internal interface IPortfolio {
        int PortfolioId { get; }
        decimal Value { get; set; }
        decimal NetProfit { get; set; }
        decimal PercentageReturn { get; set; }
        Dictionary<string, IPosition>? Positions { get; set; }

        public void RemoveStocks(string stockSymbol, int quantity);
        public void AddStocks(string stockSymbol, int quantity, decimal pricePerStock);
    }
}
