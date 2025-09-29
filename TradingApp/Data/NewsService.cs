using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace TradingApp.Data
{
    public class NewsService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly string _apiKey;

        public NewsService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
            _apiKey = config["AlphaVantageApiKey"] ?? 
                throw new ArgumentNullException(nameof(config), "AlphaVantageApiKey not configured");
        }

        public async Task<List<NewsArticle>> GetMarketNewsAsync()
        {
            try
            {
                var url = $"https://www.alphavantage.co/query?function=NEWS_SENTIMENT&topics=technology,earnings&apikey={_apiKey}";
                var response = await _http.GetFromJsonAsync<AlphaVantageResponse>(url);
                
                return response?.Feed
                    .Select(item => new NewsArticle
                    {
                        Title = item.Title,
                        Description = item.Summary,
                        Url = item.Url,
                        Source = item.Source,
                        PublishedAt = DateTime.Parse(item.TimePublished),
                        ImageUrl = item.BannerImage
                    })
                    .ToList() ?? new List<NewsArticle>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching news: {ex.Message}");
                return new List<NewsArticle>();
            }
        }
    }

    public class NewsArticle
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string Source { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? ImageUrl { get; set; }
    }

    internal class AlphaVantageResponse
    {
        [JsonPropertyName("feed")]
        public List<NewsItem> Feed { get; set; } = new();
    }

    internal class NewsItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;
        
        [JsonPropertyName("banner_image")]
        public string BannerImage { get; set; } = string.Empty;
        
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
        
        [JsonPropertyName("time_published")]
        public string TimePublished { get; set; } = string.Empty;
    }
}