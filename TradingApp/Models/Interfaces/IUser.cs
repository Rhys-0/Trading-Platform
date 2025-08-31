namespace TradingApp.Models.Interfaces {
    public interface IUser {
        public void AddCash(decimal amount);
        public void RemoveCash(decimal amount);
    }
}
