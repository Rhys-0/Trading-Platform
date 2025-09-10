using System.Linq.Expressions;
using TradingApp.Models.Interfaces;

namespace TradingApp.Models {
    internal sealed class User : IUser {
        public long Id { get; }
        public string Username { get; } // No set needed unless users being able to change their username is implemented
        public string Email { get; } // No set needed unless users being able to change their emails is implemented
        public string FirstName { get; }
        public string LastName { get; }
        public decimal StartingCashBalance { get; }
        public decimal CurrentCashBalance { get; private set; }
        public IPortfolio? Portfolio { get; private set; }
        public List<ITrade>? Trades { get; private set; }

        internal User(long id, string username, string email, string firstName, string lastName, decimal startingCashBalance, decimal currentCashBalance) {
            ArgumentOutOfRangeException.ThrowIfNegative(startingCashBalance);
            
            Id = id;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            StartingCashBalance = startingCashBalance;
            CurrentCashBalance = currentCashBalance;
        }

        public void LoadTrades() {
            Trades = null;
            throw new NotImplementedException();
        }

        public void LoadPortfolio() {
            Portfolio = null;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add cash to the users balance
        /// </summary>
        /// <param name="amount">The amount to be added to the user balance</param>
        /// <returns>True if the cash was addedd successfully, false otherwise</returns>
        public bool AddCash(decimal amount) {
            if(amount < 0) {
                return false;
            }

            CurrentCashBalance += amount;
            return true;
        }

        /// <summary>
        /// Remove cash from the users balance
        /// </summary>
        /// <param name="amount">The amount to be removed from the user balance</param>
        /// <returns>True if the cash was removed successfully, false if the operation is invalid</returns>
        public bool RemoveCash(decimal amount) {
            if(amount > CurrentCashBalance || amount < 0) {
                return false;
            }

            CurrentCashBalance -= amount;
            return true;
        }
    }
}
