using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Auth.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<AuthTokenDto>;
