using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.ServiceRequests.Queries.GetServiceRequestById;

public record GetServiceRequestByIdQuery(Guid RequestId, Guid? RequestingUserId, string RequestingUserRole) : IRequest<ServiceRequestDto>;
