using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Commands.AddProviderService;

public sealed class AddProviderServiceCommandHandler(
    IServiceProviderRepository providerRepo,
    IUnitOfWork uow)
    : IRequestHandler<AddProviderServiceCommand, ProviderServiceListingDto>
{
    public async Task<ProviderServiceListingDto> Handle(AddProviderServiceCommand request, CancellationToken ct)
    {
        var provider = await providerRepo.GetByIdAsync(request.ProviderId, ct)
            ?? throw new NotFoundException("Provider not found.");

        var added = provider.AddService(request.CategoryId, request.Name, request.Description, request.BasePrice);
        await providerRepo.UpdateAsync(provider, ct);
        await uow.SaveChangesAsync(ct);

        return new ProviderServiceListingDto(added.Id, added.Name, added.Description, added.BasePrice.Amount, added.CategoryId);
    }
}
