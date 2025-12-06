using Microsoft.Playwright;
using VoyadoSearchEngine.Server.Engines;
using VoyadoSearchEngine.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin() // Only in dev
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IBrowser>(sp =>
{
    var playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
    return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = true
    }).GetAwaiter().GetResult();
});

// Search engines & service
builder.Services.AddScoped<ISearchEngine, BingEngine>();
builder.Services.AddScoped<ISearchEngine, WikidataEngine>();
builder.Services.AddScoped<ISearchEngine, OpenLibraryEngine>();

builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();