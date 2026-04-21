using MediatR;

namespace ServiceMatch.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public record CancelServiceRequestCommand(Guid RequestId, Guid ClientId) : IRequest;
