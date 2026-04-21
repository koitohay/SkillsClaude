using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequests;

public record GetServiceRequestsQuery(Guid ClientId) : IRequest<IReadOnlyList<ServiceRequestDto>>;
