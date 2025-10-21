using Microsoft.Extensions.Hosting;
using TradingApp.BackgroundServices;
using TradingApp.Components;
using TradingApp.Data;
using TradingApp.Data.Interfaces;
using TradingApp.Models;
using TradingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Singletons
builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddSingleton<NewsService>();
builder.Services.AddSingleton<Stocks>();

// Scoped classes
builder.Services.AddScoped<ILoginManager, LoginManager>();
builder.Services.AddSingleton<UserManager>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<PortfolioService>();

// Register HttpClient for Stock News API calls
builder.Services.AddHttpClient<NewsService>(client =>
{
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
});
builder.Services.AddScoped<NewsService>();

// Background Services
builder.Services.AddHostedService<StockPriceService>();
builder.Services.AddHostedService<PortfolioUpdateService>();

// Authentication service
builder.Services.AddScoped<TradingApp.Data.Interfaces.IAuthenticationService, TradingApp.Data.AuthenticationService>();

// Email and verification services removed - not needed for core functionality

// Leaderboard service
builder.Services.AddScoped<TradingApp.Models.Interfaces.ILeaderboardService, TradingApp.Data.LeaderboardService>();

// Sample data service
builder.Services.AddScoped<TradingApp.Data.SampleDataService>();

var app = builder.Build();

// Seed sample data on startup - DISABLED to prevent database connection errors
// using (var scope = app.Services.CreateScope()) {
//     var sampleDataService = scope.ServiceProvider.GetRequiredService<TradingApp.Data.SampleDataService>();
//     await sampleDataService.SeedSampleUsersAsync();
// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    app.UseHsts();
}

// Disable HTTPS redirection for clean development startup
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<TradingApp.Components.App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
