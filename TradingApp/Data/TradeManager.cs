using Dapper;
using TradingApp.Data.Interfaces;
using TradingApp.Models;

namespace TradingApp.Data {
    internal class TradeManager : ITradeManager {
        private readonly DatabaseConnection _connection;

        public TradeManager(DatabaseConnection connection) {
            _connection = connection;
        }

        public async Task<bool> AddTradeAsync(Trade trade) {
            const string sql = @"
                INSERT INTO trades (trade_type, stock_symbol, quantity, price, time)
                VALUES (@TradeType, @StockSymbol, @Quantity, @Price, @Time);";

            using var conn = await _connection.CreateConnectionAsync();
            var rows = await conn.ExecuteAsync(sql, trade);
            return rows == 1;
        }

        public async Task<IEnumerable<Trade>> GetAllTradesAsync() {
            const string sql = @"
                SELECT
                    trade_id     AS TradeId,
                    trade_type   AS TradeType,
                    stock_symbol AS StockSymbol,
                    quantity     AS Quantity,
                    price        AS Price,
                    time         AS Time
                FROM trades
                ORDER BY time DESC;";

            using var conn = await _connection.CreateConnectionAsync();
            return await conn.QueryAsync<Trade>(sql);
        }

        public async Task<IEnumerable<Trade>> GetTradesByUserIdAsync(long userId) {
            const string sql = @"
                SELECT
                    trade_id     AS TradeId,
                    trade_type   AS TradeType,
                    stock_symbol AS StockSymbol,
                    quantity     AS Quantity,
                    price        AS Price,
                    time         AS Time
                FROM trades
                WHERE user_id = @UserId
                ORDER BY time DESC;";

            using var conn = await _connection.CreateConnectionAsync();
            return await conn.QueryAsync<Trade>(sql, new { UserId = userId });
        }
    }
}