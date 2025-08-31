namespace TradingApp.Models.Interfaces {
    internal interface IUser {
        public void AddCash(decimal amount);
        public void RemoveCash(decimal amount);
    }
}
