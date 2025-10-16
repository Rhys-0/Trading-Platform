namespace TradingApp.Models {
    public class LeaderboardEntry {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public decimal StartingBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal TotalValue { get; set; } // Current balance + portfolio value
        public decimal NetProfit { get; set; } // Total profit/loss amount
        public decimal PercentageReturn { get; set; } // Percentage gain/loss
        public int Rank { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

