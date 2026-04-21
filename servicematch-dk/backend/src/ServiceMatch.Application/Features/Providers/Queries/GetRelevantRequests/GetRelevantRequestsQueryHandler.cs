using MediatR;
using ServiceMatch.Application.Common.Mappers;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Providers.Queries.GetRelevantRequests;

public sealed class GetRelevantRequestsQueryHandler(IServiceRequestRepository requestRepo)
    : IRequestHandler<GetRelevantRequestsQuery, IReadOnlyList<ServiceRequestDto>>
{
    public async Task<IReadOnlyList<ServiceRequestDto>> Handle(GetRelevantRequestsQuery request, CancellationToken ct)
    {
        var requests = await requestRepo.GetRelevantForProviderAsync(request.ProviderId, ct);
        return requests.Select(ServiceRequestMapper.ToDto).ToList();
    }
}
