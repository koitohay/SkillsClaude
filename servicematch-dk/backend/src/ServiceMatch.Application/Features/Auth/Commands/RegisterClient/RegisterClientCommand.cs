using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterClient;

public record RegisterClientCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<ClientDto>;
