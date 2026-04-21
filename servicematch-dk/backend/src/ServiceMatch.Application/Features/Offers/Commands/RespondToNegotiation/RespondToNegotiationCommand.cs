using MediatR;

namespace ServiceMatch.Application.Features.Offers.Commands.RespondToNegotiation;

public enum NegotiationResponse { Accept, Decline }

public record RespondToNegotiationCommand(
    Guid ServiceRequestId,
    Guid OfferId,
    Guid NegotiationId,
    NegotiationResponse Response) : IRequest;
