namespace TradingApp.Models;

public class Stock
{
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
}
