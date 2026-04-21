using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Profile.Commands.AddProviderService;

public record AddProviderServiceCommand(
    Guid ProviderId,
    int? CategoryId,
    string Name,
    string Description,
    decimal BasePrice) : IRequest<ProviderServiceListingDto>;
