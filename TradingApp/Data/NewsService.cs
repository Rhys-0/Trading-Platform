using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TradingApp.Data
{
    public class NewsService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public NewsService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["AlphaVantageApiKey"] ?? 
                throw new ArgumentNullException(nameof(config), "AlphaVantageApiKey not configured");
        }

        public async Task<List<NewsArticle>> GetMarketNewsAsync()
        {
            try
            {
                var relativeUrl = $"query?function=NEWS_SENTIMENT&topics=technology,earnings&apikey={_apiKey}";
                var requestUri = new Uri(relativeUrl, UriKind.Relative);
                var response = await _http.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                    return new List<NewsArticle>();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                
                // Guard against non-JSON payloads (rate limits often return HTML/text)
                var span = jsonString.AsSpan().TrimStart();
                if (span.Length == 0 || (span[0] != '{' && span[0] != '['))
                {
                    Console.WriteLine("AlphaVantage returned non-JSON payload (likely rate limit reached).");
                    return new List<NewsArticle>();
                }

                AlphaVantageResponse? apiResponse;
                try
                {
                    apiResponse = JsonSerializer.Deserialize<AlphaVantageResponse>(jsonString, _jsonOptions);
                }
                catch (JsonException ex)
                {
                    var sample = jsonString.Length > 300 ? jsonString[..300] + "..." : jsonString;
                    Console.WriteLine($"Error parsing JSON: {ex.Message}. Payload sample: {sample}");
                    return new List<NewsArticle>();
                }

                var newsItems = apiResponse?.Feed ?? new List<NewsItem>();

                return newsItems.Select(item => new NewsArticle
                {
                    Title = item.Title ?? "No Title",
                    Description = item.Summary ?? item.Description ?? "",
                    Url = item.Url ?? "#",
                    Source = item.Source ?? "Unknown",
                    PublishedAt = ParseAlphaVantageDateTime(item.TimePublished ?? item.PublishedAtAlternative),
                    ImageUrl = item.BannerImage
                }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error: {ex.Message}");
                return new List<NewsArticle>();
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Request timed out: {ex.Message}");
                return new List<NewsArticle>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in NewsService: {ex.Message}");
                return new List<NewsArticle>();
            }
        }

        private static DateTime ParseAlphaVantageDateTime(string? timePublished)
        {
            if (string.IsNullOrEmpty(timePublished))
                return DateTime.UtcNow;

            // Format is "yyyyMMddTHHmmss"
            if (DateTime.TryParseExact(timePublished, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            
            // Fallback for other date formats
            if (DateTime.TryParse(timePublished, out result))
            {
                return result;
            }

            return DateTime.UtcNow;
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

    public class AlphaVantageResponse
    {
        public List<NewsItem>? Feed { get; set; }
        
        // Alpha Vantage returns this as a string (e.g., "50"), not a number or array
        [JsonPropertyName("items")]
        public string? Items { get; set; }
    }

    public class NewsItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Summary { get; set; }
        [JsonPropertyName("banner_image")]
        public string? BannerImage { get; set; }
        public string? Source { get; set; }
        [JsonPropertyName("time_published")]
        public string? TimePublished { get; set; }
        public string? Description { get; set; }
        [JsonPropertyName("published_at")]
        public string? PublishedAtAlternative { get; set; }
    }
}