using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Offers.Commands.AcceptOffer;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Offers;

public class AcceptOfferCommandHandlerTests
{
    private readonly IServiceRequestRepository _requestRepo = Substitute.For<IServiceRequestRepository>();
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IServiceProviderRepository _providerRepo = Substitute.For<IServiceProviderRepository>();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private AcceptOfferCommandHandler CreateHandler() =>
        new(_requestRepo, _clientRepo, _providerRepo, _emailService, _uow);

    [Fact]
    public async Task Handle_AcceptsOfferAndSendsEmails()
    {
        var clientId = Guid.NewGuid();
        var request = ServiceRequest.Create(clientId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        var offer = request.AddOffer(Guid.NewGuid(), 500, null);

        _requestRepo.GetByIdWithOffersAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);
        _clientRepo.GetByIdAsync(clientId, Arg.Any<CancellationToken>())
            .Returns(Client.Create("Client", "client@example.com", "+4520123456", "hash"));
        _providerRepo.GetByIdAsync(offer.ServiceProviderId, Arg.Any<CancellationToken>())
            .Returns(Domain.Entities.ServiceProvider.Create(
                "Salon", "Lars", "lars@salon.dk", "+4521234567", "Addr", "Aarhus", "12345678", "hash"));

        var command = new AcceptOfferCommand(request.Id, offer.Id, clientId);
        await CreateHandler().Handle(command, default);

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _emailService.Received(2).SendConfirmationAsync(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenClientDoesNotOwnRequest_ThrowsDomainException()
    {
        var ownerId = Guid.NewGuid();
        var wrongClientId = Guid.NewGuid();
        var request = ServiceRequest.Create(ownerId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        var offer = request.AddOffer(Guid.NewGuid(), 500, null);

        _requestRepo.GetByIdWithOffersAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var command = new AcceptOfferCommand(request.Id, offer.Id, wrongClientId);
        var act = async () => await CreateHandler().Handle(command, default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*authorised*");
    }
}
