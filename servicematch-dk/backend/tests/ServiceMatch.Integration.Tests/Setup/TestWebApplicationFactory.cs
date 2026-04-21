using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceMatch.Infrastructure.Persistence;

namespace ServiceMatch.Integration.Tests.Setup;

public class TestWebApplicationFactory(string connectionString)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-min-32-chars-long-replace-in-prod",
                ["Jwt:Issuer"] = "servicematch-api",
                ["Jwt:Audience"] = "servicematch-clients",
                ["Email:Provider"] = "logging",
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseNpgsql(connectionString));
        });
    }
}
