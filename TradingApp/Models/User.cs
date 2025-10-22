using System.Linq.Expressions;


namespace TradingApp.Models {
    public class User {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal StartingCashBalance { get; set; }
        public decimal CurrentCashBalance { get; set; }
        public Portfolio? Portfolio { get; set; }
        public List<Trade>? Trades { get; set; }

        // Parameterless constructor for Dapper
        public User() { }

        public User(int id, string username, string email, string firstName, string lastName, decimal startingCashBalance, decimal currentCashBalance) {
            ArgumentOutOfRangeException.ThrowIfNegative(startingCashBalance);
            
            Id = id;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            StartingCashBalance = startingCashBalance;
            CurrentCashBalance = currentCashBalance;
        }

        

        /// <summary>
        /// Add cash to the users balance
        /// </summary>
        /// <param name="amount">The amount to be added to the user balance</param>
        /// <returns>True if the cash was addedd successfully, false otherwise</returns>
        public bool AddCash(decimal amount) {
            if(amount <= 0) {
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
