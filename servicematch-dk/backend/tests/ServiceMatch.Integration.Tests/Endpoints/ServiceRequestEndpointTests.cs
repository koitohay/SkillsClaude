using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ServiceMatch.Integration.Tests.Setup;

namespace ServiceMatch.Integration.Tests.Endpoints;

public class ServiceRequestEndpointTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateRequest_WithValidData_Returns201AndIsRetrievable()
    {
        await GetClientTokenAsync();

        var response = await Client.PostAsJsonAsync("/api/v1/requests", new
        {
            categoryId = 1,
            requestedDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
            requestedTime = "10:00",
            city = "Aarhus"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResp = await Client.GetAsync("/api/v1/requests");
        listResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await listResp.Content.ReadAsStringAsync();
        body.Should().Contain("Aarhus");
    }

    [Fact]
    public async Task CreateRequest_Unauthenticated_Returns401()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var response = await Client.PostAsJsonAsync("/api/v1/requests", new
        {
            categoryId = 1,
            requestedDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
            requestedTime = "10:00",
            city = "Aarhus"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
