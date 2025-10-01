using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;
using TradingApp.Data.Interfaces;

namespace TradingApp.Data {
    public class DatabaseConnection : IDatabaseConnection {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseConnection>? _logger;

        public DatabaseConnection(IConfiguration configuration, ILogger<DatabaseConnection>? logger = null) {
            // Use configuration instead of hardcoded string
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                "User Id=postgres.hnpkbxsqvfzvkhedwpas;Password=DgsOAEF2lXntu8nb;Server=aws-1-ap-southeast-2.pooler.supabase.com;Port=5432;Database=postgre;SSL Mode=Require;Trust Server Certificate=true;";
        
            _logger = logger;
            Console.WriteLine($"🔍 DB DEBUG: Connection string loaded: {_connectionString}");
            _logger?.LogInformation("Database connection string loaded: {ConnectionString}", _connectionString);
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
