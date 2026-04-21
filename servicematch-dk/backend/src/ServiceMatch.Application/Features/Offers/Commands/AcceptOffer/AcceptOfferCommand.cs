using MediatR;

namespace ServiceMatch.Application.Features.Offers.Commands.AcceptOffer;

public record AcceptOfferCommand(Guid ServiceRequestId, Guid OfferId, Guid ClientId) : IRequest;
