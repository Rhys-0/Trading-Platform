using TradingApp.Models;

namespace TradingApp.Models.Interfaces {
    public interface ILeaderboardService {
        Task<List<LeaderboardEntry>> GetLeaderboardAsync();
        Task<List<LeaderboardEntry>> GetLeaderboardSortedByGainsAsync();
        Task<List<LeaderboardEntry>> GetLeaderboardSortedByLossesAsync();
        Task<LeaderboardEntry?> GetUserRankAsync(int userId);
        Task<List<LeaderboardEntry>> GetTopPerformersAsync(int count = 10);
        Task<List<LeaderboardEntry>> GetWorstPerformersAsync(int count = 10);
    }
}

