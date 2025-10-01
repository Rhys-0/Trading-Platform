using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace TradingApp.Data {
    public class DatabaseConnection {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseConnection>? _logger;

        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection>? logger = null) {
            // FIXED: Using the exact Supabase connection string format that works
            _connectionString = "Host=aws-1-ap-southeast-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.hnpkbxsqvfzvkhedwpas;Password=2VY2UnyYu01dyPpA;SSL Mode=Require;Trust Server Certificate=true;";
        
            _logger = logger;
            _logger?.LogInformation("Database connection string loaded: {ConnectionString}", _connectionString);
        }

        public async Task<IDbConnection> CreateConnectionAsync() {
            try {
                var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger?.LogDebug("Database connection opened successfully");
                return connection;
            } catch (Exception ex) {
                _logger?.LogCritical("Failed to open database connection: {Ex}", ex);
                throw;
            }
        }
    }
}
