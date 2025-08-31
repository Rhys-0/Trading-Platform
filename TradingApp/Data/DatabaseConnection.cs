using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace TradingApp.Data {
    public class DatabaseConnection {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseConnection>? _logger;

        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection>? logger = null) {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found, add it to your appsettings.json");

            // Put this under "AllowedHosts" in appsettings.json

            // "ConnectionStrings": {
            //     "DefaultConnection": ""
            // }
        
            _logger = logger;
        }

        public async Task<IDbConnection> CreateConnectionAsync() {
            try {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger?.LogDebug("Database connection opened successfully");
                return connection;
            } catch (Exception ex) {
                _logger?.LogCritical($"Failed to open database connection: {ex}");
                _logger?.LogInformation(_connectionString);
                throw;
            }
        }
    }
}
