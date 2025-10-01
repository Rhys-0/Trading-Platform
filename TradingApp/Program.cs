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
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PortfolioService>();

// Background Services
builder.Services.AddHostedService<StockPriceService>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Disable HTTPS redirection for clean development startup
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<TradingApp.Components.App>()
    .AddInteractiveServerRenderMode();

    // Use a different port to avoid conflicts
    app.Urls.Add("http://localhost:3000");

// Display the application URL in the terminal
Console.WriteLine("🚀 TradingApp is starting...");
Console.WriteLine("🌐 Application URL: http://localhost:3000");
Console.WriteLine("📱 Open your browser and navigate to the URL above");
Console.WriteLine("");

await app.RunAsync();
