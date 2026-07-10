using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace IdeasClinicInventory.Tests;

public sealed class HealthEndpointTests : IClassFixture<InventoryWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(InventoryWebApplicationFactory factory)
    {
        // The in-memory test server exercises the real HTTP pipeline without
        // opening a browser or reserving a local TCP port.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Liveness_endpoint_reports_healthy()
    {
        using var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}

public sealed class InventoryWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            // Tests must not require permission to write to the Windows Event Log.
            // CI agents and other restricted accounts commonly lack that access.
            logging.ClearProviders();
        });
    }
}
