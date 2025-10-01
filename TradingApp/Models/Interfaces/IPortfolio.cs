namespace TradingApp.Models.Interfaces {
    public interface IPortfolio {
        int PortfolioId { get; }
        decimal Value { get; set; }
        decimal NetProfit { get; set; }
        decimal PercentageReturn { get; set; }
        List<IPosition>? Positions { get; set; }

        internal void LoadPositions();
    }
}
