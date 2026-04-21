using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Providers.Queries.GetRelevantRequests;

public record GetRelevantRequestsQuery(Guid ProviderId) : IRequest<IReadOnlyList<ServiceRequestDto>>;
