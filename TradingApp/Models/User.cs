using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal class User : IUser {
        public int Id { get; }
        public string Username { get; } // No set needed unless users being able to change their username is implemented
        public string Email { get; } // No set needed unless users being able to change their emails is implemented
        public string FirstName { get; }
        public string LastName { get; }
        public decimal StartingCashBalance { get; }
        public decimal CurrentCashBalance { get; private set; }
        public IPortfolio? Portfolio { get; private set; }
        public List<ITrade>? Trades { get; set; }

        internal User(int id, string username, string email, string firstName, string lastName, decimal startingCashBalance, decimal currentCashBalance) {
            Id = id;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            StartingCashBalance = startingCashBalance;
            CurrentCashBalance = currentCashBalance;
        }

        public void LoadTrades() {
            throw new NotImplementedException();
        }

        public void LoadPortfolio() {
            throw new NotImplementedException();
        }
    }
}
