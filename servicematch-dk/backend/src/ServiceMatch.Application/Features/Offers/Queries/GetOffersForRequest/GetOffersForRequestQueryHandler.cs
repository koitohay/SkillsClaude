using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Queries.GetOffersForRequest;

public sealed class GetOffersForRequestQueryHandler(IServiceRequestRepository requestRepo)
    : IRequestHandler<GetOffersForRequestQuery, IReadOnlyList<OfferDto>>
{
    public async Task<IReadOnlyList<OfferDto>> Handle(GetOffersForRequestQuery request, CancellationToken ct)
    {
        var serviceRequest = await requestRepo.GetByIdWithOffersAsync(request.ServiceRequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        return serviceRequest.Offers.Select(o => new OfferDto(
            o.Id, o.ServiceRequestId, o.ServiceProviderId, o.Price.Amount,
            o.Message, o.Status, o.CreatedAt,
            o.Negotiations.Select(n => new NegotiationDto(
                n.Id, n.InitiatedBy, n.ProposedPrice.Amount, n.Message, n.Status, n.CreatedAt
            )).ToList())).ToList();
    }
}
