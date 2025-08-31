using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    public class User : IUser {
        public int Id { get; }
        public string Username { get; } // No set needed unless users being able to change their username is implemented
        public string Email { get; } // No set needed unless users being able to change their emails is implemented
        public string FirstName { get; }
        public string LastName { get; } 
        public decimal StartingCashBalance { get; }
        public decimal CurrentCashBalance { get; private set; }

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
