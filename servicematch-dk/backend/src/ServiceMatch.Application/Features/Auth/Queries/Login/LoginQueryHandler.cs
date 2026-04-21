using MediatR;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Auth.Queries.Login;

public sealed class LoginQueryHandler(
    IClientRepository clientRepo,
    IServiceProviderRepository providerRepo,
    IPasswordHasher passwordHasher,
    IJwtService jwtService)
    : IRequestHandler<LoginQuery, AuthTokenDto>
{
    public async Task<AuthTokenDto> Handle(LoginQuery request, CancellationToken ct)
    {
        var client = await clientRepo.GetByEmailAsync(request.Email, ct);
        if (client is not null && passwordHasher.Verify(request.Password, client.PasswordHash))
            return new AuthTokenDto(jwtService.GenerateToken(client.Id, "Client"), "Client", client.Id);

        var provider = await providerRepo.GetByEmailAsync(request.Email, ct);
        if (provider is not null && passwordHasher.Verify(request.Password, provider.PasswordHash))
            return new AuthTokenDto(jwtService.GenerateToken(provider.Id, "Provider"), "Provider", provider.Id);

        // If no user found, still run verify to prevent timing attacks
        if (client == null && provider == null)
        {
            BCrypt.Net.BCrypt.Verify(request.Password, BCrypt.Net.BCrypt.HashPassword("dummy"));
            throw new DomainException("Invalid email or password.");
        }

        throw new DomainException("Invalid email or password.");
    }
}
