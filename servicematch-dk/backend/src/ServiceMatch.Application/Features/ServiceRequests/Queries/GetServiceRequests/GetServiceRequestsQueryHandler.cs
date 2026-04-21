using MediatR;
using ServiceMatch.Application.Common.Mappers;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequests;

public sealed class GetServiceRequestsQueryHandler(IServiceRequestRepository requestRepo)
    : IRequestHandler<GetServiceRequestsQuery, IReadOnlyList<ServiceRequestDto>>
{
    public async Task<IReadOnlyList<ServiceRequestDto>> Handle(GetServiceRequestsQuery request, CancellationToken ct)
    {
        var requests = await requestRepo.GetByClientIdAsync(request.ClientId, ct);
        return requests.Select(ServiceRequestMapper.ToDto).ToList();
    }
}
