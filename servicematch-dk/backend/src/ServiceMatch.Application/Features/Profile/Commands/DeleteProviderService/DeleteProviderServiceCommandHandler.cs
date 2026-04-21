using MediatR;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Commands.DeleteProviderService;

public sealed class DeleteProviderServiceCommandHandler(
    IServiceProviderRepository providerRepo,
    IUnitOfWork uow)
    : IRequestHandler<DeleteProviderServiceCommand>
{
    public async Task Handle(DeleteProviderServiceCommand request, CancellationToken ct)
    {
        var provider = await providerRepo.GetByIdAsync(request.ProviderId, ct)
            ?? throw new NotFoundException("Provider not found.");

        provider.RemoveService(request.ServiceId);
        await providerRepo.UpdateAsync(provider, ct);
        await uow.SaveChangesAsync(ct);
    }
}
