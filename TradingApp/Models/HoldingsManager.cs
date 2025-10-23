using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradingApp.Models
{
    public class HoldingsManager
    {
        public List<Holding> Holdings { get; private set; } = new();

        public decimal TotalValue => Holdings.Sum(h => h.CurrentPrice * h.Quantity);
        public decimal FlatChange => Holdings.Sum(h => (h.CurrentPrice - h.AveragePrice) * h.Quantity);
        public decimal PercentageChange => TotalValue != 0 ? FlatChange / (TotalValue - FlatChange) : 0;
        
        public void LoadHoldings(Dictionary<string, Position> positions, IDictionary<string, Stock> stockList)
        {
            if (positions == null || stockList == null)
            {
                Holdings.Clear();
                return;
            }

            Holdings = positions.Values
                .Select(p =>
                {
                    if (!stockList.TryGetValue(p.StockSymbol, out var stock))
                        return null;

                    decimal avgPrice = p.PurchaseLots?.Any() == true
                        ? p.PurchaseLots.Average(lot => lot.PurchasePrice)
                        : 0;

                    decimal gainLoss = avgPrice != 0 ? (stock.Price - avgPrice) / avgPrice : 0;

                    return new Holding
                    {
                        Ticker = p.StockSymbol,
                        Name = stock.Name,
                        Quantity = p.TotalQuantity,
                        AveragePrice = avgPrice,
                        CurrentPrice = stock.Price,
                        GainLoss = gainLoss
                    };
                })
                .Where(h => h != null)
                .ToList()!;

            var totalValue = TotalValue;
            foreach (var h in Holdings)
            {
                h.PortfolioWeight = totalValue != 0 ? (h.CurrentPrice * h.Quantity) / totalValue : 0;
            }
        }
    }

    public class Holding
    {
        public string Ticker { get; set; } = "";
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal GainLoss { get; set; }
        public decimal PortfolioWeight { get; set; }
    }
}
