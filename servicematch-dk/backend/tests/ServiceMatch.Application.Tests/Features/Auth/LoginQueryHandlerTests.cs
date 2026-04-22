using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Auth.Queries.Login;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Auth;

public class LoginQueryHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IServiceProviderRepository _providerRepo = Substitute.For<IServiceProviderRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtService _jwt = Substitute.For<IJwtService>();

    private LoginQueryHandler CreateHandler() => new(_clientRepo, _providerRepo, _hasher, _jwt);

    [Fact]
    public async Task Handle_ValidClientCredentials_ReturnsClientToken()
    {
        var client = Client.Create("Jane", "jane@example.com", "+4520123456", "hashed");
        _clientRepo.GetByEmailAsync("jane@example.com", Arg.Any<CancellationToken>()).Returns(client);
        _hasher.Verify("pass", "hashed").Returns(true);
        _jwt.GenerateToken(client.Id, "Client").Returns("token-abc");

        var result = await CreateHandler().Handle(new LoginQuery("jane@example.com", "pass"), default);

        result.Token.Should().Be("token-abc");
        result.Role.Should().Be("Client");
        result.UserId.Should().Be(client.Id);
    }

    [Fact]
    public async Task Handle_ValidProviderCredentials_ReturnsProviderToken()
    {
        _clientRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Client?)null);
        var provider = ServiceProvider.Create("Salon", "Lars", "lars@salon.dk", "+4521234567", "Addr", "København", "12345678", "hashed");
        _providerRepo.GetByEmailAsync("lars@salon.dk", Arg.Any<CancellationToken>()).Returns(provider);
        _hasher.Verify("pass", "hashed").Returns(true);
        _jwt.GenerateToken(provider.Id, "Provider").Returns("provider-token");

        var result = await CreateHandler().Handle(new LoginQuery("lars@salon.dk", "pass"), default);

        result.Role.Should().Be("Provider");
        result.Token.Should().Be("provider-token");
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsDomainException()
    {
        var client = Client.Create("Jane", "jane@example.com", "+4520123456", "hashed");
        _clientRepo.GetByEmailAsync("jane@example.com", Arg.Any<CancellationToken>()).Returns(client);
        _hasher.Verify("wrong", "hashed").Returns(false);
        _providerRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ServiceProvider?)null);

        var act = async () => await CreateHandler().Handle(new LoginQuery("jane@example.com", "wrong"), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Invalid*");
    }

    [Fact]
    public async Task Handle_UnknownEmail_ThrowsDomainException()
    {
        _clientRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Client?)null);
        _providerRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((ServiceProvider?)null);
        _hasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var act = async () => await CreateHandler().Handle(new LoginQuery("ghost@example.com", "pass"), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Invalid*");
    }
}
