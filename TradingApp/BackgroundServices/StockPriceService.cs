using Microsoft.Extensions.Configuration;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TradingApp.Models;

namespace TradingApp.BackgroundServices {
    internal class StockPriceService : BackgroundService {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly Stocks _stocks;

        public StockPriceService(ILogger<StockPriceService> logger, IConfiguration configuration, Stocks stocks) {
            _logger = logger;
            _configuration = configuration;
            _stocks = stocks;

            var apiToken = _configuration.GetConnectionString("ApiToken");
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // Initalise default stock value
            foreach (Stock stock in _stocks.StockList.Values) {
                Uri url = new($"https://finnhub.io/api/v1/quote?symbol={stock.Symbol}&token={apiToken}");
                HttpResponseMessage? response = httpClient.GetAsync(url).Result;
                string responseStr = response.Content.ReadAsStringAsync().Result;
                using JsonDocument doc = JsonDocument.Parse(responseStr);
                decimal currentPrice = doc.RootElement.GetProperty("c").GetDecimal();

                stock.Price = currentPrice;
                _logger.LogInformation("{Stock}: ${Price}", stock.Symbol, stock.Price);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            // Get WSS api key from configuration
            string? websocketUri = _configuration.GetConnectionString("WebsocketConnection");

            // initialise websocket connection to stock price service
            using var ws = new ClientWebSocket();
            Uri uri = new(websocketUri!);
            await ws.ConnectAsync(uri, CancellationToken.None);

            _logger.LogInformation("Websocket connected");

            // Subscribe to all stocks
            string msg = "";
            foreach (string ticker in _stocks.StockList.Keys) {
                msg = "{\"type\":\"subscribe\",\"symbol\":\"" + ticker + "\"}";
                _logger.LogInformation("{Msg}", msg);
                var bytes = Encoding.UTF8.GetBytes(msg);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }


            var buffer = new byte[1024 * 4];
            while (!stoppingToken.IsCancellationRequested) {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close) {
                    _logger.LogWarning("Websocket closed");
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);

                } else if (result.MessageType == WebSocketMessageType.Text) {
                    // Handle possible fragmentation
                    var message = new StringBuilder();
                    message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    while (!result.EndOfMessage) {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }

                    // parse to json and update
                    using JsonDocument doc = JsonDocument.Parse(message.ToString());

                    decimal price = doc.RootElement.GetProperty("data")[0].GetProperty("p").GetDecimal();
                    string? symbol = doc.RootElement.GetProperty("data")[0].GetProperty("s").GetString();

                    _logger.LogInformation("Recieved price update: {Symbol} ${Price}", symbol, price);
                    _stocks.StockList[symbol!].Price = price;
                }
            }
            _logger.LogError("StockPriceService is stopping");
        }
    }
}
