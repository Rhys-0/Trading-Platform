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

// Register HttpClient for Stock News API calls
builder.Services.AddHttpClient<NewsService>();
builder.Services.AddScoped<NewsService>();

// Background Services
builder.Services.AddHostedService<StockPriceService>();

var app = builder.Build();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
