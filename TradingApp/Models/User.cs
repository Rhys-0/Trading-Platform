using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal class User : IUser {
        internal int Id { get; }
        internal string Username { get; } // No set needed unless users being able to change their username is implemented
        internal string Email { get; } // No set needed unless users being able to change their emails is implemented
        internal string FirstName { get; }
        internal string LastName { get; }
        internal decimal StartingCashBalance { get; }
        internal decimal CurrentCashBalance { get; private set; }

        // to add
        // current positions
        // portfolio statistics

        public User(int id, string username, string email, string firstName, string lastName, decimal startingCashBalance, decimal currentCashBalance) {
            Id = id;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            StartingCashBalance = startingCashBalance;
            CurrentCashBalance = currentCashBalance;
        }

        public void AddCash(decimal amount) {
            throw new NotImplementedException();
        }

        public void RemoveCash(decimal amount) { 
            throw new NotImplementedException(); 
        } 
    }
}
