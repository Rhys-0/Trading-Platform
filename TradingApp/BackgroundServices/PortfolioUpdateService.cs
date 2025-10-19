using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TradingApp.Data;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using TradingApp.Services;

namespace TradingApp.BackgroundServices {
    internal class PortfolioUpdateService : BackgroundService {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly Stocks _stocks;
        private readonly PortfolioService _portfolioService;
        private readonly UserManager _userManager;

        public PortfolioUpdateService(ILogger<StockPriceService> logger, IConfiguration configuration, Stocks stocks, UserManager userManager, PortfolioService portfolioService) {
            _logger = logger;
            _configuration = configuration;
            _stocks = stocks;
            _portfolioService = portfolioService;
            _userManager = userManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                var users = await _userManager.GetAllUsers();
                foreach (User user in users) {
                    _portfolioService.UpdateUserPortfolio(user);
                    await _userManager.UpdatePortfolio(user.Portfolio!);

                    await Task.Delay(60000, stoppingToken);
                }
            }
        }
    }
}
