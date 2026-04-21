using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Offers.Queries.GetOffersForRequest;

public record GetOffersForRequestQuery(Guid ServiceRequestId) : IRequest<IReadOnlyList<OfferDto>>;
