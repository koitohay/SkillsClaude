using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Offers.Commands.SubmitOffer;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Offers;

public class SubmitOfferCommandHandlerTests
{
    private readonly IServiceRequestRepository _requestRepo = Substitute.For<IServiceRequestRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private SubmitOfferCommandHandler CreateHandler() => new(_requestRepo, _uow);

    private static ServiceRequest MakeOpenRequest(Guid clientId) =>
        ServiceRequest.Create(clientId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");

    [Fact]
    public async Task Handle_ValidOffer_ReturnsOfferDto()
    {
        var request = MakeOpenRequest(Guid.NewGuid());
        var providerId = Guid.NewGuid();
        _requestRepo.GetByIdWithOffersAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var command = new SubmitOfferCommand(request.Id, providerId, 500, "Vi kan hjælpe");
        var result = await CreateHandler().Handle(command, default);

        result.Price.Should().Be(500);
        result.ServiceProviderId.Should().Be(providerId);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_RequestNotFound_ThrowsNotFoundException()
    {
        _requestRepo.GetByIdWithOffersAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((ServiceRequest?)null);

        var act = async () => await CreateHandler().Handle(
            new SubmitOfferCommand(Guid.NewGuid(), Guid.NewGuid(), 500, null), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
