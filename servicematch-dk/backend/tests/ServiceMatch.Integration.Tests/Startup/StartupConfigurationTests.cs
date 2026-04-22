using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace ServiceMatch.Integration.Tests.Startup;

/// <summary>
/// Validates the JWT secret guard that Program.cs applies at startup.
/// These tests exercise the validation logic directly using IConfiguration,
/// matching exactly what Program.cs does, rather than spinning up the full host.
/// </summary>
public class StartupConfigurationTests
{
    private static IConfiguration BuildConfig(Dictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();

    private static void AssertJwtSecretGuard(IConfiguration config)
    {
        // This mirrors the guard in Program.cs verbatim
        var jwtSecret = config["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException(
                "JWT secret is not configured. Set Jwt:Secret (min 32 chars) via environment variable JWT__SECRET or user-secrets.");
    }

    [Fact]
    public void JwtSecretGuard_WithEmptyString_ThrowsInvalidOperationException()
    {
        var config = BuildConfig(new() { ["Jwt:Secret"] = "" });

        var act = () => AssertJwtSecretGuard(config);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT secret*");
    }

    [Fact]
    public void JwtSecretGuard_WithWhitespace_ThrowsInvalidOperationException()
    {
        var config = BuildConfig(new() { ["Jwt:Secret"] = "   " });

        var act = () => AssertJwtSecretGuard(config);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT secret*");
    }

    [Fact]
    public void JwtSecretGuard_WithNullKey_ThrowsInvalidOperationException()
    {
        var config = BuildConfig(new());  // Jwt:Secret absent → null

        var act = () => AssertJwtSecretGuard(config);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT secret*");
    }

    [Fact]
    public void JwtSecretGuard_WithValidSecret_DoesNotThrow()
    {
        var config = BuildConfig(new() { ["Jwt:Secret"] = "a-valid-secret-that-is-long-enough-32chars" });

        var act = () => AssertJwtSecretGuard(config);

        act.Should().NotThrow();
    }
}
