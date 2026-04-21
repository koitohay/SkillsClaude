using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Commands.CounterOffer;

public sealed class CounterOfferCommandHandler(
    IOfferRepository offerRepo,
    IUnitOfWork uow)
    : IRequestHandler<CounterOfferCommand, NegotiationDto>
{
    public async Task<NegotiationDto> Handle(CounterOfferCommand request, CancellationToken ct)
    {
        var offer = await offerRepo.GetByIdWithNegotiationsAsync(request.OfferId, ct)
            ?? throw new NotFoundException("Offer not found.");

        if (offer.ServiceRequestId != request.ServiceRequestId)
            throw new DomainException("Offer does not belong to this service request.");

        var lastNegotiation = offer.Negotiations
            .OrderByDescending(n => n.CreatedAt)
            .FirstOrDefault();

        var expectedInitiator = lastNegotiation is null
            ? NegotiationInitiator.Client
            : (lastNegotiation.InitiatedBy == NegotiationInitiator.Client
                ? NegotiationInitiator.Provider
                : NegotiationInitiator.Client);

        var callerInitiator = request.InitiatorRole == "Client"
            ? NegotiationInitiator.Client
            : NegotiationInitiator.Provider;

        if (callerInitiator != expectedInitiator)
            throw new DomainException("Not your turn to counter.");

        var negotiation = offer.AddCounterOffer(request.InitiatedBy, request.ProposedPrice, request.Message);
        await offerRepo.UpdateAsync(offer, ct);
        await uow.SaveChangesAsync(ct);

        return new NegotiationDto(
            negotiation.Id, negotiation.InitiatedBy, negotiation.ProposedPrice.Amount,
            negotiation.Message, negotiation.Status, negotiation.CreatedAt);
    }
}
