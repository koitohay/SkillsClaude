using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Features.ServiceRequests.Commands.CreateServiceRequest;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.ServiceRequests;

public class CreateServiceRequestCommandHandlerTests
{
    private readonly IServiceRequestRepository _repo = Substitute.For<IServiceRequestRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private CreateServiceRequestCommandHandler CreateHandler() => new(_repo, _uow);

    private static DateOnly Tomorrow => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

    [Fact]
    public async Task Handle_WithValidCategory_CreatesRequest()
    {
        var command = new CreateServiceRequestCommand(
            Guid.NewGuid(), 1, null, Tomorrow, new TimeOnly(10, 0), "Aarhus");

        var result = await CreateHandler().Handle(command, default);

        result.Status.Should().Be(ServiceRequestStatus.Open);
        result.CategoryId.Should().Be(1);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithPastDate_ThrowsDomainException()
    {
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var command = new CreateServiceRequestCommand(
            Guid.NewGuid(), 1, null, yesterday, new TimeOnly(10, 0), "Aarhus");

        var act = async () => await CreateHandler().Handle(command, default);
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_WithNoCategory_ThrowsDomainException()
    {
        var command = new CreateServiceRequestCommand(
            Guid.NewGuid(), null, null, Tomorrow, new TimeOnly(10, 0), "Aarhus");

        var act = async () => await CreateHandler().Handle(command, default);
        await act.Should().ThrowAsync<DomainException>();
    }
}
