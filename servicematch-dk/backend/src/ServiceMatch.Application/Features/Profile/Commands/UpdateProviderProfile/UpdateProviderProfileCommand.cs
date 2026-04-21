using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateProviderProfile;

public record UpdateProviderProfileCommand(
    Guid ProviderId,
    string CompanyName,
    string ContactName,
    string Email,
    string PhoneNumber,
    string Address,
    string City) : IRequest<ServiceProviderDto>;
