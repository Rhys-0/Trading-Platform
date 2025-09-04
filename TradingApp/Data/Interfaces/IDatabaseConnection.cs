using System.Data;

namespace TradingApp.Data.Interfaces {
    internal interface IDatabaseConnection {
        public Task<IDbConnection> CreateConnectionAsync();
    }
}
