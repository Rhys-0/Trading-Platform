namespace TradingApp.Models.Interfaces {
    public interface IUser {
        int Id { get; }
        string Username { get; } // No set needed unless users being able to change their username is implemented
        string Email { get; } // No set needed unless users being able to change their emails is implemented
        string FirstName { get; }
        string LastName { get; }
        decimal StartingCashBalance { get; }
        decimal CurrentCashBalance { get; }
        IPortfolio? Portfolio { get; }
        List<ITrade>? Trades { get; set; }

        internal void LoadTrades();
        internal void LoadPortfolio();
    }
}
