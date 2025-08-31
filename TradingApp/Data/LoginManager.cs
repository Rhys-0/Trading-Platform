using Npgsql;
using Dapper;
using TradingApp.Data.Interfaces;
using TradingApp.Models;

namespace TradingApp.Data {
    public class LoginManager : ILoginManager {
        private readonly DatabaseConnection _connection;
        public LoginManager(DatabaseConnection connection) {
            _connection = connection;
        }

        /// <summary>
        /// Return the a user object given the email and hashed password of a user, used to validate user logins.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="hashedPassword">The password of the user</param>
        /// <returns>A User object containing all the information from the user table in the database,
        /// if no user with the given email and password is found, the function returns null.</returns>
        public async Task<User?> RetrieveUser(string email, string hashedPassword) {
            using var connection = await _connection.CreateConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT user_id, username, email, first_name, last_name, starting_cash_balance, current_cash_balance " +
                "FROM users " +
                "WHERE email = @Email and password_hash = @HashedPassword",
                new { Email = email, HashedPassword = hashedPassword });
        }

        /// <summary>
        /// Add a user to the database, typically on the registration page.
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="email">The email of the user</param>
        /// <param name="passwordHash">The password of the user</param>
        /// <param name="firstName">The first name of the user</param>
        /// <param name="lastName">The surname of the user</param>
        /// <returns>True if the user was added successfully, false otherwise</returns>
        public async Task<bool> AddUser(string username, string email, string passwordHash, string firstName, string lastName) {
            decimal startingCash = 10_000.00m;

            using var connection = await _connection.CreateConnectionAsync();
            int rowsAffected = await connection.ExecuteAsync(
                "INSERT into users" +
                "(username, email, password_hash, first_name, last_name, starting_cash_balance, current_cash_balance) values" +
                "(@Username, @Email, @PasswordHash, @FirstName, @LastName, @StartingCashBalance, @CurrentCashBalance)",
                new {Username = username, 
                    Email = email,
                    PasswordHash = passwordHash, 
                    FirstName = firstName,
                    LastName = lastName, 
                    StartingCashBalance = startingCash, 
                    CurrentCashBalance = startingCash}
                );

            return (rowsAffected == 1);
        }


    }
}
