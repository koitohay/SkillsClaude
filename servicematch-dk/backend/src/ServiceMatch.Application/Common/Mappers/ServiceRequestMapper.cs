using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Entities;

namespace ServiceMatch.Application.Common.Mappers;

public static class ServiceRequestMapper
{
    public static ServiceRequestDto ToDto(ServiceRequest r) => new(
        r.Id, r.ClientId, r.CategoryId, r.Category?.Name,
        r.FreeTextDescription, r.RequestedDate, r.RequestedTime, r.City.Name, r.Status, r.CreatedAt,
        r.Offers.Select(o => new OfferDto(
            o.Id, o.ServiceRequestId, o.ServiceProviderId, o.Price.Amount,
            o.Message, o.Status, o.CreatedAt,
            o.Negotiations.Select(n => new NegotiationDto(
                n.Id, n.InitiatedBy, n.ProposedPrice.Amount, n.Message, n.Status, n.CreatedAt
            )).ToList()
        )).ToList());
}
