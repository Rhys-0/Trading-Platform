namespace TradingApp.Models.Interfaces {
    internal interface IUser {
        long Id { get; }
        string Username { get; } // No set needed unless users being able to change their username is implemented
        string Email { get; } // No set needed unless users being able to change their emails is implemented
        string FirstName { get; }
        string LastName { get; }
        decimal StartingCashBalance { get; }
        decimal CurrentCashBalance { get; }
        IPortfolio? Portfolio { get; set; }
        List<ITrade>? Trades { get; set; }

        internal bool AddCash(decimal amount);
        internal bool RemoveCash(decimal amount);
    }
}
