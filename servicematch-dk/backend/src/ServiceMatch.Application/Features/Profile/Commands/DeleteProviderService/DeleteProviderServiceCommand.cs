using MediatR;

namespace ServiceMatch.Application.Features.Profile.Commands.DeleteProviderService;

public record DeleteProviderServiceCommand(Guid ProviderId, Guid ServiceId) : IRequest;
