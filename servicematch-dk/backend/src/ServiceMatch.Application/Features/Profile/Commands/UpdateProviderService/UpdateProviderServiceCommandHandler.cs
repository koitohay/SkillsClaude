using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateProviderService;

public sealed class UpdateProviderServiceCommandHandler(
    IServiceProviderRepository providerRepo,
    IUnitOfWork uow)
    : IRequestHandler<UpdateProviderServiceCommand, ProviderServiceListingDto>
{
    public async Task<ProviderServiceListingDto> Handle(UpdateProviderServiceCommand request, CancellationToken ct)
    {
        var provider = await providerRepo.GetByIdAsync(request.ProviderId, ct)
            ?? throw new NotFoundException("Provider not found.");

        provider.UpdateService(request.ServiceId, request.Name, request.Description, request.BasePrice);
        await providerRepo.UpdateAsync(provider, ct);
        await uow.SaveChangesAsync(ct);

        var updated = provider.Services.First(s => s.Id == request.ServiceId);
        return new ProviderServiceListingDto(updated.Id, updated.Name, updated.Description, updated.BasePrice.Amount, updated.CategoryId);
    }
}
