using MediatR;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Commands.DeclineOffer;

public sealed class DeclineOfferCommandHandler(
    IServiceRequestRepository requestRepo,
    IUnitOfWork uow)
    : IRequestHandler<DeclineOfferCommand>
{
    public async Task Handle(DeclineOfferCommand request, CancellationToken ct)
    {
        var serviceRequest = await requestRepo.GetByIdWithOffersAsync(request.ServiceRequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        if (serviceRequest.ClientId != request.ClientId)
            throw new DomainException("You are not authorised to decline offers on this request.");

        var offer = serviceRequest.Offers.FirstOrDefault(o => o.Id == request.OfferId)
            ?? throw new NotFoundException("Offer not found.");

        offer.Decline();
        await requestRepo.UpdateAsync(serviceRequest, ct);
        await uow.SaveChangesAsync(ct);
    }
}
