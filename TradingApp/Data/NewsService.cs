using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
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
                
                var response = await _http.GetAsync(url);
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw API Response: {jsonString}"); // Debug logging

                // First try to parse as a dynamic object to inspect the structure
                using JsonDocument document = JsonDocument.Parse(jsonString);
                var root = document.RootElement;

                // Check if we have a feed or items property
                var newsArray = root.TryGetProperty("feed", out var feedElement) 
                    ? feedElement 
                    : root.TryGetProperty("items", out var itemsElement) 
                        ? itemsElement 
                        : default;

                if (newsArray.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine("Response does not contain a valid news array");
                    return new List<NewsArticle>();
                }

                var articles = new List<NewsArticle>();
                
                foreach (var item in newsArray.EnumerateArray())
                {
                    try
                    {
                        var article = new NewsArticle
                        {
                            Title = GetJsonString(item, "title"),
                            Description = GetJsonString(item, "summary") ?? GetJsonString(item, "description") ?? "",
                            Url = GetJsonString(item, "url"),
                            Source = GetJsonString(item, "source"),
                            PublishedAt = ParseDateTime(GetJsonString(item, "time_published") ?? GetJsonString(item, "published_at") ?? ""),
                            ImageUrl = GetJsonString(item, "banner_image")
                        };
                        
                        if (!string.IsNullOrEmpty(article.Title))
                        {
                            articles.Add(article);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing news item: {ex.Message}");
                        continue;
                    }
                }

                return articles;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching news: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return new List<NewsArticle>();
            }
        }

        private static DateTime ParseDateTime(string dateTimeStr)
        {
            return DateTime.TryParse(dateTimeStr, out var result) 
                ? result 
                : DateTime.UtcNow;
        }

        private static string? GetJsonString(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
                ? property.GetString()
                : null;
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
        public List<NewsItem>? Feed { get; set; }

        [JsonPropertyName("items")]
        public List<NewsItem>? Items { get; set; }
    }

    internal class NewsItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("summary")]
        public string? Summary { get; set; }
        
        [JsonPropertyName("banner_image")]
        public string? BannerImage { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;
        
        [JsonPropertyName("time_published")]
        public string? TimePublished { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("published_at")]
        public string? PublishedAtAlternative { get; set; }
    }
}