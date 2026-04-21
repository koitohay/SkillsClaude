using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Commands.SubmitOffer;

public sealed class SubmitOfferCommandHandler(
    IServiceRequestRepository requestRepo,
    IUnitOfWork uow)
    : IRequestHandler<SubmitOfferCommand, OfferDto>
{
    public async Task<OfferDto> Handle(SubmitOfferCommand request, CancellationToken ct)
    {
        var serviceRequest = await requestRepo.GetByIdWithOffersAsync(request.ServiceRequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        var offer = serviceRequest.AddOffer(request.ProviderId, request.Price, request.Message);
        await requestRepo.UpdateAsync(serviceRequest, ct);
        await uow.SaveChangesAsync(ct);

        return MapToDto(offer);
    }

    private static OfferDto MapToDto(Domain.Entities.Offer o) =>
        new(o.Id, o.ServiceRequestId, o.ServiceProviderId, o.Price.Amount,
            o.Message, o.Status, o.CreatedAt, []);
}
