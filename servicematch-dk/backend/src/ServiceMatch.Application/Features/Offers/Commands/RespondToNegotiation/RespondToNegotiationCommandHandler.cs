using MediatR;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Commands.RespondToNegotiation;

public sealed class RespondToNegotiationCommandHandler(
    IOfferRepository offerRepo,
    IUnitOfWork uow)
    : IRequestHandler<RespondToNegotiationCommand>
{
    public async Task Handle(RespondToNegotiationCommand request, CancellationToken ct)
    {
        var offer = await offerRepo.GetByIdWithNegotiationsAsync(request.OfferId, ct)
            ?? throw new NotFoundException("Offer not found.");

        if (offer.ServiceRequestId != request.ServiceRequestId)
            throw new DomainException("Offer does not belong to this service request.");

        var negotiation = offer.Negotiations.FirstOrDefault(n => n.Id == request.NegotiationId)
            ?? throw new NotFoundException("Negotiation not found.");

        if (request.Response == NegotiationResponse.Accept)
            negotiation.Accept();
        else
            negotiation.Decline();

        await offerRepo.UpdateAsync(offer, ct);
        await uow.SaveChangesAsync(ct);
    }
}
