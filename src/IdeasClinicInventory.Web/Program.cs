using IdeasClinicInventory.Web.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages keeps the first version server-rendered and avoids introducing a
// separate JavaScript application before the inventory workflows require one.
builder.Services.AddRazorPages();

var inventoryConnectionString = builder.Configuration.GetConnectionString("InventoryDatabase")
    ?? string.Empty;

if (string.IsNullOrWhiteSpace(inventoryConnectionString))
{
    throw new InvalidOperationException(
        "Connection string 'InventoryDatabase' was not found. " +
        "Configure it in appsettings.Development.json or through deployment secrets.");
}

// Keeping the provider configuration here lets the DbContext remain independent
// of a particular environment. Production can replace only the connection string.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(inventoryConnectionString));

builder.Services
    .AddHealthChecks()
    // The self check proves that the web process can respond without requiring SQL.
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
        tags: ["live"])
    // The readiness check opens a real database connection when /health/ready is called.
    .AddDbContextCheck<ApplicationDbContext>("sql-server", tags: ["ready"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

// Hosting platforms can use the lightweight endpoint to decide whether the app
// process is alive, and the readiness endpoint before sending it user traffic.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

app.Run();

// WebApplicationFactory locates this entry point when running integration tests.
// It has no effect on the application's runtime behavior.
public partial class Program;
