using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using System.Runtime.CompilerServices;
using TradingApp.Data;
using Xunit;

namespace TradingApp.Tests.Data {
    public class DatabaseConnectionTests : IClassFixture<MockDatabase>{
        private readonly MockDatabase _db;
        public DatabaseConnectionTests(MockDatabase db) => _db = db;

        [Fact]
        public async Task CreateConnectionAsync_ShouldOpenConnection() {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", _db.ConnectionString}
                })
                .Build();

            var mock = new Mock<ILogger<DatabaseConnection>>();
            ILogger<DatabaseConnection> logger = mock.Object;
            var dbConnection = new DatabaseConnection(config, logger);

            // Act
            using var connection = await dbConnection.CreateConnectionAsync();

            // Assert
            Assert.NotNull(connection);
            Assert.Equal(System.Data.ConnectionState.Open, connection.State);

            mock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database connection opened successfully")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateConnectionAsync_WithInvalidParams_ShouldThrowException() {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                    {"ConnectionStrings:DefaultConnection", "Malformed connection string"}
                })
                .Build();

            var mock = new Mock<ILogger<DatabaseConnection>>();
            ILogger<DatabaseConnection> logger = mock.Object;
            var dbConnection = new DatabaseConnection(config, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ConnectionAbortedException>(async () => await dbConnection.CreateConnectionAsync());
        }

        [Fact]
        public void Constructor_WithNoConnectionString_ShouldThrowException() {
            // Arrange empty config without connection string
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> {
                })
                .Build();

            var mock = new Mock<ILogger<DatabaseConnection>>();
            ILogger<DatabaseConnection> logger = mock.Object;

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => new DatabaseConnection(config, logger));
        }

    }


}
