using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
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
builder.Services.AddSingleton<Stocks>();
builder.Services.AddSingleton<ILoginManager, LoginManager>();  // Changed from Scoped to Singleton
builder.Services.AddSingleton<UserManager>();
builder.Services.AddSingleton<PortfolioService>();

// Scoped classes
builder.Services.AddScoped<UserService>();

// Register HttpClient for Stock News API calls (only this registration)
builder.Services.AddHttpClient<NewsService>(client =>
{
    client.BaseAddress = new Uri("https://www.alphavantage.co/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("TradingApp/1.0");
});

// Background Services
builder.Services.AddHostedService<StockPriceService>();
builder.Services.AddHostedService<PortfolioUpdateService>();

// Authentication service - Register as Singleton so the state persists
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

// Authentication State Provider - MUST be scoped for Blazor
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
{
    var customAuthStateProvider = provider.GetRequiredService<CustomAuthenticationStateProvider>();
    var authService = (AuthenticationService)provider.GetRequiredService<IAuthenticationService>();

    // Connect the two services
    authService.SetAuthStateProvider(customAuthStateProvider);

    return customAuthStateProvider;
});

// Add Blazor authorization
builder.Services.AddCascadingAuthenticationState();

// Leaderboard service
builder.Services.AddScoped<TradingApp.Models.Interfaces.ILeaderboardService, TradingApp.Data.LeaderboardService>();

// Sample data service
builder.Services.AddScoped<TradingApp.Data.SampleDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<TradingApp.Components.App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();