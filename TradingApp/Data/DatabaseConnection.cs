using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using TradingApp.Data.Interfaces;

namespace TradingApp.Data {
    internal class DatabaseConnection : IDatabaseConnection {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseConnection>? _logger;

        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection>? logger = null) {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found, add it to your appsettings.json");
        
            _logger = logger;
        }

        public async Task<IDbConnection> CreateConnectionAsync() {
            try {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger?.LogInformation("Database connection opened successfully");
                return connection;
            } catch (Exception ex) {
                _logger?.LogCritical(ex, "Failed to open database connection: {Ex}", ex);
                throw new ConnectionAbortedException("Failed to open database connection");
            }
        }
    }
}
