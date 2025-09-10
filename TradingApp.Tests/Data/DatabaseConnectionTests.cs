using Moq;
using Xunit;
using Microsoft.Extensions.Logging.Testing;
using TradingApp.Data;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

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
    }


}
