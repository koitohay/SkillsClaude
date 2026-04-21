using MediatR;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Offers.Commands.AcceptOffer;

public sealed class AcceptOfferCommandHandler(
    IServiceRequestRepository requestRepo,
    IClientRepository clientRepo,
    IServiceProviderRepository providerRepo,
    IEmailService emailService,
    IUnitOfWork uow)
    : IRequestHandler<AcceptOfferCommand>
{
    public async Task Handle(AcceptOfferCommand request, CancellationToken ct)
    {
        var serviceRequest = await requestRepo.GetByIdWithOffersAsync(request.ServiceRequestId, ct)
            ?? throw new NotFoundException("Service request not found.");

        if (serviceRequest.ClientId != request.ClientId)
            throw new DomainException("You are not authorised to accept offers on this request.");

        var offer = serviceRequest.Offers.FirstOrDefault(o => o.Id == request.OfferId)
            ?? throw new NotFoundException("Offer not found.");

        serviceRequest.AcceptOffer(request.OfferId);
        await requestRepo.UpdateAsync(serviceRequest, ct);
        await uow.SaveChangesAsync(ct);

        await SendConfirmationEmailsAsync(request.ClientId, offer.ServiceProviderId, ct);
    }

    private async Task SendConfirmationEmailsAsync(Guid clientId, Guid providerId, CancellationToken ct)
    {
        var client = await clientRepo.GetByIdAsync(clientId, ct);
        var provider = await providerRepo.GetByIdAsync(providerId, ct);

        if (client is not null)
            await emailService.SendConfirmationAsync(
                client.Email.Value, client.FullName,
                "Your service booking is confirmed!",
                $"Great news! Your service request has been accepted by {provider?.CompanyName}.", ct);

        if (provider is not null)
            await emailService.SendConfirmationAsync(
                provider.Email.Value, provider.ContactName,
                "You have a new booking!",
                $"You have been selected for a service request. The client will be in touch.", ct);
    }
}
