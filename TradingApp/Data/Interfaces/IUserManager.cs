namespace TradingApp.Data.Interfaces {
    internal interface IUserManager {
        public void SetBalance(int userID, decimal amount);
    }
}
