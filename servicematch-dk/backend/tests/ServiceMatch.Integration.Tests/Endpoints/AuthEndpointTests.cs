using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ServiceMatch.Integration.Tests.Setup;

namespace ServiceMatch.Integration.Tests.Endpoints;

public class AuthEndpointTests : IntegrationTestBase
{
    [Fact]
    public async Task RegisterClient_WithValidData_Returns201()
    {
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register/client", new
        {
            fullName = "Jane Doe",
            email = "jane@example.dk",
            phoneNumber = "+4520111222",
            password = "Password1!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("id");
    }

    [Fact]
    public async Task RegisterClient_WithDuplicateEmail_Returns400()
    {
        var payload = new
        {
            fullName = "Jane Doe",
            email = "duplicate@example.dk",
            phoneNumber = "+4520111223",
            password = "Password1!"
        };
        await Client.PostAsJsonAsync("/api/v1/auth/register/client", payload);
        var response = await Client.PostAsJsonAsync("/api/v1/auth/register/client", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        await Client.PostAsJsonAsync("/api/v1/auth/register/client", new
        {
            fullName = "Login User",
            email = "loginuser@example.dk",
            phoneNumber = "+4520111224",
            password = "Password1!"
        });

        var resp = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "loginuser@example.dk",
            password = "Password1!"
        });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("token");
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns400()
    {
        var resp = await Client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "nobody@example.dk",
            password = "wrongpassword"
        });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
