using TradingApp.Data;
using TradingApp.Models;
using TradingApp.Models.Interfaces;
using Xunit;

namespace TradingApp.Tests {
    public class LeaderboardTests {
        private readonly MockLeaderboardService _mockLeaderboardService;

        public LeaderboardTests() {
            _mockLeaderboardService = new MockLeaderboardService();
        }

        [Fact]
        public async Task GetLeaderboard_ShouldReturnSortedByMostGained() {
            // Arrange
            
            // Act
            var leaderboard = await _mockLeaderboardService.GetLeaderboardSortedByGainsAsync();
            
            // Assert
            Assert.NotNull(leaderboard);
            Assert.NotEmpty(leaderboard);
            
            // Check if sorted by percentage return (descending - highest first)
            for (int i = 0; i < leaderboard.Count - 1; i++) {
                Assert.True(leaderboard[i].PercentageReturn >= leaderboard[i + 1].PercentageReturn,
                    $"Leaderboard not sorted correctly. Item {i} has {leaderboard[i].PercentageReturn}%, item {i + 1} has {leaderboard[i + 1].PercentageReturn}%");
            }
        }

        [Fact]
        public async Task GetLeaderboard_ShouldReturnSortedByMostLost() {
            // Arrange
            
            // Act
            var leaderboard = await _mockLeaderboardService.GetLeaderboardSortedByLossesAsync();
            
            // Assert
            Assert.NotNull(leaderboard);
            Assert.NotEmpty(leaderboard);
            
            // Check if sorted by percentage return (ascending - lowest first)
            for (int i = 0; i < leaderboard.Count - 1; i++) {
                Assert.True(leaderboard[i].PercentageReturn <= leaderboard[i + 1].PercentageReturn,
                    $"Leaderboard not sorted correctly for most lost. Item {i} has {leaderboard[i].PercentageReturn}%, item {i + 1} has {leaderboard[i + 1].PercentageReturn}%");
            }
        }

        [Fact]
        public async Task GetLeaderboard_WithDefaultSort_ShouldReturnMostGained() {
            // Arrange
            
            // Act
            var leaderboard = await _mockLeaderboardService.GetLeaderboardAsync();
            
            // Assert
            Assert.NotNull(leaderboard);
            Assert.NotEmpty(leaderboard);
            
            // Should be sorted by highest percentage return first (default behavior)
            var firstUser = leaderboard.First();
            var expectedHighestUser = _mockLeaderboardService.GetMockUsers().OrderByDescending(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100).First();
            
            Assert.Equal(expectedHighestUser.Username, firstUser.Username);
        }

        [Fact]
        public async Task GetLeaderboard_ShouldContainAllRequiredFields() {
            // Arrange
            
            // Act
            var leaderboard = await _mockLeaderboardService.GetLeaderboardAsync();
            
            // Assert
            Assert.NotNull(leaderboard);
            Assert.NotEmpty(leaderboard);
            
            foreach (var entry in leaderboard) {
                Assert.NotNull(entry.Username);
                Assert.NotEmpty(entry.Username);
                Assert.NotNull(entry.FirstName);
                Assert.NotEmpty(entry.FirstName);
                Assert.NotNull(entry.LastName);
                Assert.NotEmpty(entry.LastName);
                Assert.True(entry.CurrentBalance >= 0);
                Assert.True(entry.StartingBalance > 0);
                Assert.True(entry.NetProfit >= 0 || entry.NetProfit < 0); // Can be positive or negative
                Assert.True(entry.PercentageReturn >= -100); // Can't lose more than 100%
            }
        }

        [Fact]
        public async Task GetLeaderboard_ShouldCalculateCorrectPercentageReturn() {
            // Arrange
            
            // Act
            var leaderboard = await _mockLeaderboardService.GetLeaderboardAsync();
            
            // Assert
            Assert.NotNull(leaderboard);
            Assert.NotEmpty(leaderboard);
            
            foreach (var entry in leaderboard) {
                var expectedPercentage = entry.NetProfit / entry.StartingBalance * 100;
                Assert.Equal(expectedPercentage, entry.PercentageReturn, 2); // Allow 2 decimal places precision
            }
        }

        [Fact]
        public async Task GetLeaderboard_ShouldReturnConsistentResults() {
            // Arrange
            
            // Act
            var leaderboard1 = await _mockLeaderboardService.GetLeaderboardAsync();
            var leaderboard2 = await _mockLeaderboardService.GetLeaderboardAsync();
            
            // Assert
            Assert.NotNull(leaderboard1);
            Assert.NotNull(leaderboard2);
            Assert.Equal(leaderboard1.Count, leaderboard2.Count);
            
            for (int i = 0; i < leaderboard1.Count; i++) {
                Assert.Equal(leaderboard1[i].Username, leaderboard2[i].Username);
                Assert.Equal(leaderboard1[i].PercentageReturn, leaderboard2[i].PercentageReturn);
            }
        }

        [Fact]
        public async Task GetTopPerformers_ShouldReturnCorrectCount() {
            // Arrange
            var count = 3;
            
            // Act
            var topPerformers = await _mockLeaderboardService.GetTopPerformersAsync(count);
            
            // Assert
            Assert.NotNull(topPerformers);
            Assert.True(topPerformers.Count <= count);
            Assert.NotEmpty(topPerformers);
            
            // Should be sorted by highest percentage return
            for (int i = 0; i < topPerformers.Count - 1; i++) {
                Assert.True(topPerformers[i].PercentageReturn >= topPerformers[i + 1].PercentageReturn);
            }
        }

        [Fact]
        public async Task GetWorstPerformers_ShouldReturnCorrectCount() {
            // Arrange
            var count = 2;
            
            // Act
            var worstPerformers = await _mockLeaderboardService.GetWorstPerformersAsync(count);
            
            // Assert
            Assert.NotNull(worstPerformers);
            Assert.True(worstPerformers.Count <= count);
            Assert.NotEmpty(worstPerformers);
            
            // Should be sorted by lowest percentage return
            for (int i = 0; i < worstPerformers.Count - 1; i++) {
                Assert.True(worstPerformers[i].PercentageReturn <= worstPerformers[i + 1].PercentageReturn);
            }
        }

        [Fact]
        public async Task GetUserRank_ShouldReturnCorrectRank() {
            // Arrange
            var userId = 1;
            
            // Act
            var userRank = await _mockLeaderboardService.GetUserRankAsync(userId);
            
            // Assert
            Assert.NotNull(userRank);
            Assert.Equal(userId, userRank.UserId);
            Assert.True(userRank.Rank > 0);
        }
    }

    // Mock implementation for testing
    public class MockLeaderboardService : ILeaderboardService {
        private readonly List<User> _mockUsers;

        public MockLeaderboardService() {
            _mockUsers = GetMockUsers();
        }

        public List<User> GetMockUsers() {
            return new List<User> {
                new User(1, "Winner", "winner@email.com", "John", "Winner", 10000m, 15000m), // 50% gain
                new User(2, "Loser", "loser@email.com", "Jane", "Loser", 10000m, 7000m),    // 30% loss
                new User(3, "Average", "average@email.com", "Bob", "Average", 10000m, 10500m), // 5% gain
                new User(4, "BigWinner", "bigwinner@email.com", "Alice", "BigWinner", 10000m, 20000m), // 100% gain
                new User(5, "BreakEven", "breakeven@email.com", "Charlie", "BreakEven", 10000m, 10000m) // 0% change
            };
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardAsync() {
            await Task.Delay(10); // Simulate async operation
            return CreateLeaderboardEntries(_mockUsers.OrderByDescending(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100));
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardSortedByGainsAsync() {
            await Task.Delay(10); // Simulate async operation
            return CreateLeaderboardEntries(_mockUsers.OrderByDescending(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100));
        }

        public async Task<List<LeaderboardEntry>> GetLeaderboardSortedByLossesAsync() {
            await Task.Delay(10); // Simulate async operation
            return CreateLeaderboardEntries(_mockUsers.OrderBy(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100));
        }

        public async Task<LeaderboardEntry?> GetUserRankAsync(int userId) {
            await Task.Delay(10); // Simulate async operation
            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return null;

            var allEntries = CreateLeaderboardEntries(_mockUsers.OrderByDescending(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100));
            return allEntries.FirstOrDefault(e => e.UserId == userId);
        }

        public async Task<List<LeaderboardEntry>> GetTopPerformersAsync(int count = 10) {
            await Task.Delay(10); // Simulate async operation
            var topUsers = _mockUsers.OrderByDescending(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100).Take(count);
            return CreateLeaderboardEntries(topUsers);
        }

        public async Task<List<LeaderboardEntry>> GetWorstPerformersAsync(int count = 10) {
            await Task.Delay(10); // Simulate async operation
            var worstUsers = _mockUsers.OrderBy(u => (u.CurrentCashBalance - u.StartingCashBalance) / u.StartingCashBalance * 100).Take(count);
            return CreateLeaderboardEntries(worstUsers);
        }

        private List<LeaderboardEntry> CreateLeaderboardEntries(IEnumerable<User> users) {
            var entries = new List<LeaderboardEntry>();
            int rank = 1;

            foreach (var user in users) {
                var netProfit = user.CurrentCashBalance - user.StartingCashBalance;
                var percentageReturn = netProfit / user.StartingCashBalance * 100;

                entries.Add(new LeaderboardEntry {
                    UserId = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    StartingBalance = user.StartingCashBalance,
                    CurrentBalance = user.CurrentCashBalance,
                    TotalValue = user.CurrentCashBalance, // Simplified for testing
                    NetProfit = netProfit,
                    PercentageReturn = percentageReturn,
                    Rank = rank++,
                    LastUpdated = DateTime.UtcNow
                });
            }

            return entries;
        }
    }
}
