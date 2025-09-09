using Moq;
using Xunit;
using Microsoft.Extensions.Logging.Testing;
using TradingApp.Data;
using Microsoft.Extensions.Configuration;

namespace TradingApp.Tests.Data {
    internal class DatabaseConnectionTests {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly FakeLogger<DatabaseConnection> _fakeLogger;
    }
}
