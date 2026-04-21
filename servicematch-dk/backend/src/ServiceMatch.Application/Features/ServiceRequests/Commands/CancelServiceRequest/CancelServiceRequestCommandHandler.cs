using MediatR;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.ServiceRequests.Commands.CancelServiceRequest;

public sealed class CancelServiceRequestCommandHandler(
    IServiceRequestRepository requestRepo,
    IUnitOfWork uow)
    : IRequestHandler<CancelServiceRequestCommand>
{
    public async Task Handle(CancelServiceRequestCommand request, CancellationToken ct)
    {
        var serviceRequest = await requestRepo.GetByIdAsync(request.RequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        if (serviceRequest.ClientId != request.ClientId)
            throw new DomainException("You are not authorised to cancel this request.");

        serviceRequest.Cancel();
        await requestRepo.UpdateAsync(serviceRequest, ct);
        await uow.SaveChangesAsync(ct);
    }
}
