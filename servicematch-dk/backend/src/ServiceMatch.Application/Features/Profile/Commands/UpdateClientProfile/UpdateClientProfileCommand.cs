using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateClientProfile;

public record UpdateClientProfileCommand(
    Guid ClientId,
    string FullName,
    string Email,
    string PhoneNumber) : IRequest<ClientDto>;
