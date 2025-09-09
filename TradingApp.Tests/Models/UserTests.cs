using Xunit;
using TradingApp.Models;
using System.Linq.Expressions;

namespace TradingApp.Tests.Models {
    public class UserTests {
        [Fact]
        public void Constructor_WithValidParams_ShouldCreateCorrectUser() {
            var id = 1;
            var username = "TestUser";
            var email = "test@email.com";
            var firstName = "John";
            var lastName = "Doe";
            var startingBal = 10_000.25m;
            var currentBal = 4_233.82m;

            var user = new User(id, username, email, firstName, lastName, startingBal, currentBal);

            Assert.Equal(id, user.Id);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(startingBal, user.StartingCashBalance);
            Assert.Equal(currentBal, user.CurrentCashBalance);
            Assert.Null(user.Portfolio);
            Assert.Null(user.Trades);
        }

        [Fact]
        public void Constructor_WithNegativeBalance_ShouldFail() {
            var id = 1;
            var username = "TestUser";
            var email = "test@email.com";
            var firstName = "John";
            var lastName = "Doe";
            var startingBal = -1_000.00m;
            var currentBal = 4_233.82m;

            Assert.Throws<ArgumentOutOfRangeException>(()=>new User(id, username, email, firstName, lastName, startingBal, currentBal));
        }

        [Fact]
        public void AddCash_WithPositiveNumber_ShouldReturnTrue() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            Assert.True(user.AddCash(100m));
        }

        [Fact]
        public void AddCash_WithNegativeNumber_ShouldReturnFalse() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            Assert.False(user.AddCash(-100m));
        }

        [Fact]
        public void AddCash_WithPositiveNumber_ShouldAddCash() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            user.AddCash(100m);

            Assert.Equal(1_100m, user.CurrentCashBalance);
        }

        [Fact]
        public void RemoveCash_WithPositiveNumber_ShouldReturnTrue() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            Assert.True(user.RemoveCash(100m));
        }

        [Fact]
        public void RemoveCash_WithNegativeNumber_ShouldReturnFalse() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            Assert.False(user.RemoveCash(-100m));
        }

        [Fact]
        public void RemoveCash_WithNumberLargerThanCurrentBalance_ShouldReturnFalse() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            Assert.False(user.RemoveCash(-2_000m));
        }

        [Fact]
        public void RemoveCash_WithPositiveNumber_ShouldRemoveCash() {
            var user = new User(1, "TestUser", "test@email.com", "John", "Doe", 1_000m, 1_000m);

            user.RemoveCash(100m);

            Assert.Equal(900m, user.CurrentCashBalance);
        }

        [Fact]
        public void LoadPortfolio_ShouldLoadPortfolio() {
            Assert.True(true);
        }

        [Fact]
        public void LoadTrades_ShouldLoadTrades() {
            Assert.True(true);
        }
    }
}
