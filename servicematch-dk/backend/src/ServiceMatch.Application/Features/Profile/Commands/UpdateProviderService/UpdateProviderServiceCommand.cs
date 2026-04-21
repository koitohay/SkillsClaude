using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateProviderService;

public record UpdateProviderServiceCommand(
    Guid ProviderId,
    Guid ServiceId,
    string Name,
    string Description,
    decimal BasePrice) : IRequest<ProviderServiceListingDto>;
