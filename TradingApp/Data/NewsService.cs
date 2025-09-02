using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradingApp.Data
{
    public class NewsArticle
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime PublishedAt { get; set; }
        public string Source { get; set; } = "";
    }

    public class NewsService
    {
        // Mock data (later hook up to real API)
        public Task<List<NewsArticle>> GetLatestNews()
        {
            var articles = new List<NewsArticle>
            {
                new NewsArticle
                {
                    Title = "Tech Stocks Rally Amid Market Optimism",
                    Description = "Major tech stocks surged today as investors showed optimism in earnings reports.",
                    Url = "https://example.com/article1",
                    PublishedAt = DateTime.UtcNow.AddHours(-2),
                    Source = "Bloomberg"
                },
                new NewsArticle
                {
                    Title = "Global Markets Await Fed Interest Rate Decision",
                    Description = "Traders are closely watching the Federal Reserve's next move on interest rates.",
                    Url = "https://example.com/article2",
                    PublishedAt = DateTime.UtcNow.AddHours(-5),
                    Source = "Reuters"
                },
                new NewsArticle
                {
                    Title = "Oil Prices Dip Amid Supply Concerns",
                    Description = "Crude oil prices saw a slight decline following new supply chain disruptions.",
                    Url = "https://example.com/article3",
                    PublishedAt = DateTime.UtcNow.AddHours(-8),
                    Source = "CNBC"
                }
            };

            return Task.FromResult(articles);
        }
    }
}