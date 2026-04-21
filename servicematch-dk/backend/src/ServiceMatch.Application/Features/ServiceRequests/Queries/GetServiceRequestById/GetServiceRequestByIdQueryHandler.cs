using MediatR;
using ServiceMatch.Application.Common.Mappers;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public sealed class GetServiceRequestByIdQueryHandler(IServiceRequestRepository requestRepo)
    : IRequestHandler<GetServiceRequestByIdQuery, ServiceRequestDto>
{
    public async Task<ServiceRequestDto> Handle(GetServiceRequestByIdQuery request, CancellationToken ct)
    {
        var r = await requestRepo.GetByIdWithOffersAsync(request.RequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        if (request.RequestingUserRole == "Client" && r.ClientId != request.RequestingUserId)
            throw new NotFoundException("Service request not found.");

        return ServiceRequestMapper.ToDto(r);
    }
}
