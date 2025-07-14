using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using VaultDynamicDbDemo;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Configuration.AddJsonFile("/vault/secrets/db-secrets.json", optional: false, reloadOnChange: true);

builder.Services.AddScoped<AppDbContext>(sp =>
{
    string connectionString = $"Host=hippo-dev-primary-service.postgresql.svc.cluster.local;Port=5432;Database=hippo;Username={builder.Configuration["username"]};Password={builder.Configuration["password"]};";
    Console.WriteLine("refreshed conn string..." + connectionString);
    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    optionsBuilder.UseNpgsql(connectionString);
    return new AppDbContext(optionsBuilder.Options);
});

builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy("Live check passed"), tags: new[] { "live" })
    .AddCheck<DynamicNpgSqlHealthCheck>("postgres", tags: new[] { "ready" });


builder.Services.AddControllers();
var app = builder.Build();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("live") });
app.MapHealthChecks("/healthz/ready", new HealthCheckOptions { Predicate = registration => registration.Tags.Contains("ready") });

app.MapGet("/", async ([FromServices] AppDbContext db, [FromServices] ILogger<Program> logger) =>
{
    try
    {
        var products = await db.Products.ToListAsync();
        return Results.Ok(products);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while fetching products.");
        return Results.Problem("fetching products An unexpected error occurred.");
    }
});


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/test", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}