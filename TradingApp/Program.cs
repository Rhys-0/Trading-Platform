using TradingApp.Data;
using TradingApp.Data.Interfaces;
using TradingApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DatabaseConnection>();

// Database manager methods
builder.Services.AddScoped<ILoginManager, LoginManager>();

// Authentication service
builder.Services.AddScoped<TradingApp.Data.Interfaces.IAuthenticationService, TradingApp.Data.AuthenticationService>();

// Email and verification services
builder.Services.AddScoped<TradingApp.Data.Interfaces.IEmailService, TradingApp.Data.EmailService>();
builder.Services.AddScoped<TradingApp.Data.Interfaces.IVerificationService, TradingApp.Data.VerificationService>();

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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<TradingApp.Components.App>()
    .AddInteractiveServerRenderMode();

// Use a different port to avoid conflicts
app.Urls.Add("http://localhost:8080");

// Display the application URL in the terminal
Console.WriteLine("🚀 TradingApp is starting...");
Console.WriteLine("🌐 Application URL: http://localhost:8080");
Console.WriteLine("📱 Open your browser and navigate to the URL above");
Console.WriteLine("");

await app.RunAsync();
