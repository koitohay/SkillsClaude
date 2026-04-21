using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public sealed class CreateServiceRequestCommandHandler(
    IServiceRequestRepository requestRepo,
    IUnitOfWork uow)
    : IRequestHandler<CreateServiceRequestCommand, ServiceRequestDto>
{
    public async Task<ServiceRequestDto> Handle(CreateServiceRequestCommand request, CancellationToken ct)
    {
        var serviceRequest = ServiceRequest.Create(
            request.ClientId,
            request.CategoryId,
            request.FreeTextDescription,
            request.RequestedDate,
            request.RequestedTime,
            request.City);

        await requestRepo.AddAsync(serviceRequest, ct);
        await uow.SaveChangesAsync(ct);

        return MapToDto(serviceRequest);
    }

    private static ServiceRequestDto MapToDto(ServiceRequest r) =>
        new(r.Id, r.ClientId, r.CategoryId, null,
            r.FreeTextDescription, r.RequestedDate, r.RequestedTime, r.City.Name, r.Status, r.CreatedAt);
}
