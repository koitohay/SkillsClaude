using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceMatch.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace ServiceMatch.Integration.Tests.Setup;

[Collection("Integration")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected HttpClient Client { get; private set; } = null!;
    protected TestWebApplicationFactory Factory { get; private set; } = null!;

    private static readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithDatabase("servicematch_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private static bool _containerStarted;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    public async Task InitializeAsync()
    {
        await _lock.WaitAsync();
        try
        {
            if (!_containerStarted)
            {
                await _postgres.StartAsync();
                _containerStarted = true;
            }
        }
        finally { _lock.Release(); }

        Factory = new TestWebApplicationFactory(_postgres.GetConnectionString());
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Truncate all tables for test isolation
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE \"Negotiations\", \"Offers\", \"ServiceRequests\", " +
            "\"ProviderCategories\", \"ServiceProviders\", \"Clients\" RESTART IDENTITY CASCADE");

        Factory.Dispose();
    }

    protected async Task<string> GetClientTokenAsync(string email = "test@client.dk", string password = "Password1!")
    {
        await Client.PostAsJsonAsync("/api/v1/auth/register/client", new
        {
            fullName = "Test Client",
            email,
            phoneNumber = "+4520123456",
            password
        });

        var resp = await Client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
        return body.Token;
    }

    protected async Task<string> GetProviderTokenAsync(string email = "provider@salon.dk", string password = "Password1!")
    {
        await Client.PostAsJsonAsync("/api/v1/auth/register/provider", new
        {
            companyName = "Test Salon",
            contactName = "Test Provider",
            email,
            phoneNumber = "+4521234567",
            address = "Main St 1",
            city = "Aarhus",
            cvrNumber = "12345678",
            password,
            categoryIds = new[] { 1 }
        });

        var resp = await Client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        var body = await resp.Content.ReadFromJsonAsync<TokenResponse>();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
        return body.Token;
    }

    private record TokenResponse(string Token, string Role, Guid UserId);
}

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<object>;
