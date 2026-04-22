using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.ServiceRequests.Commands.CancelServiceRequest;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.ServiceRequests;

public class CancelServiceRequestCommandHandlerTests
{
    private readonly IServiceRequestRepository _requestRepo = Substitute.For<IServiceRequestRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private CancelServiceRequestCommandHandler CreateHandler() => new(_requestRepo, _uow);

    [Fact]
    public async Task Handle_OwnerCancels_SavesChanges()
    {
        var clientId = Guid.NewGuid();
        var request = ServiceRequest.Create(clientId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        await CreateHandler().Handle(new CancelServiceRequestCommand(request.Id, clientId), default);

        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        request.Status.ToString().Should().Be("Cancelled");
    }

    [Fact]
    public async Task Handle_WrongClient_ThrowsDomainException()
    {
        var ownerId = Guid.NewGuid();
        var request = ServiceRequest.Create(ownerId, 1, null,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(10, 0), "Aarhus");
        _requestRepo.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(request);

        var act = async () => await CreateHandler().Handle(
            new CancelServiceRequestCommand(request.Id, Guid.NewGuid()), default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*authorised*");
    }

    [Fact]
    public async Task Handle_RequestNotFound_ThrowsNotFoundException()
    {
        _requestRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((ServiceRequest?)null);

        var act = async () => await CreateHandler().Handle(
            new CancelServiceRequestCommand(Guid.NewGuid(), Guid.NewGuid()), default);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
