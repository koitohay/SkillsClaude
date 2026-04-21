using MediatR;

namespace ServiceMatch.Application.Features.Offers.Commands.DeclineOffer;

public record DeclineOfferCommand(Guid ServiceRequestId, Guid OfferId, Guid ClientId) : IRequest;
