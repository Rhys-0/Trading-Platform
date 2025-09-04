using TradingApp.Components;
using TradingApp.Data;
using TradingApp.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Singletons
builder.Services.AddSingleton<DatabaseConnection>();
builder.Services.AddSingleton<NewsService>();

// Scoped classes
builder.Services.AddScoped<ILoginManager, LoginManager>();

// Register HttpClient for Stock News API calls
builder.Services.AddHttpClient<NewsService>();
builder.Services.AddScoped<NewsService>();

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
