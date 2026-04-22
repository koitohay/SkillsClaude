using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Offers.Commands.DeclineOffer;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Offers;

public class DeclineOfferCommandHandlerTests
{
    private readonly IServiceRequestRepository _requestRepo = Substitute.For<IServiceRequestRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private DeclineOfferCommandHandler CreateHandler() => new(_requestRepo, _uow);

    [Fact]
    public async Task Handle_ValidDecline_SavesChanges()
    {
        var clientId = Guid.NewGuid();
        var request = ServiceRequest.Create(clientId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        var offer = request.AddOffer(Guid.NewGuid(), 500, null);
        _requestRepo.GetByIdWithOffersAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        await CreateHandler().Handle(new DeclineOfferCommand(request.Id, offer.Id, clientId), default);

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        offer.Status.ToString().Should().Be("Declined");
    }

    [Fact]
    public async Task Handle_WrongClient_ThrowsDomainException()
    {
        var ownerId = Guid.NewGuid();
        var request = ServiceRequest.Create(ownerId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        var offer = request.AddOffer(Guid.NewGuid(), 500, null);
        _requestRepo.GetByIdWithOffersAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var act = async () => await CreateHandler().Handle(
            new DeclineOfferCommand(request.Id, offer.Id, Guid.NewGuid()), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*authorised*");
    }
}
