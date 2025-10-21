// Data/Interfaces/ITradeManager.cs
using TradingApp.Models;

namespace TradingApp.Data.Interfaces {
    public interface ITradeManager {
        Task<bool> AddTradeAsync(Trade trade);
        Task<IEnumerable<Trade>> GetTradesByUserIdAsync(long userId);
        Task<IEnumerable<Trade>> GetAllTradesAsync();
    }
}
