using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Auth.Commands.RegisterProvider;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Auth;

public class RegisterProviderCommandHandlerTests
{
    private readonly IServiceProviderRepository _providerRepo = Substitute.For<IServiceProviderRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private RegisterProviderCommandHandler CreateHandler() =>
        new(_providerRepo, _passwordHasher, _uow);

    private static RegisterProviderCommand ValidCommand() =>
        new("Salon Co", "Lars", "lars@salon.dk", "+4520123456",
            "Main St 1", "Aarhus", "12345678", "Password1!", [1, 2]);

    [Fact]
    public async Task Handle_WithValidInput_CreatesProviderAndReturnsDto()
    {
        _providerRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Domain.Entities.ServiceProvider?)null);
        _providerRepo.GetByCvrAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Domain.Entities.ServiceProvider?)null);
        _passwordHasher.Hash("Password1!").Returns("hashed");

        var result = await CreateHandler().Handle(ValidCommand(), default);

        result.Email.Should().Be("lars@salon.dk");
        result.CvrNumber.Should().Be("12345678");
        result.CategoryIds.Should().BeEquivalentTo(new[] { 1, 2 });
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsDomainException()
    {
        var existing = Domain.Entities.ServiceProvider.Create(
            "Other Co", "Person", "lars@salon.dk", "+4520123456", "Addr", "Aarhus", "87654321", "hash");
        _providerRepo.GetByEmailAsync("lars@salon.dk", Arg.Any<CancellationToken>()).Returns(existing);
        _providerRepo.GetByCvrAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Domain.Entities.ServiceProvider?)null);

        var act = async () => await CreateHandler().Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*already registered*");
    }

    [Fact]
    public async Task Handle_WithDuplicateCvr_ThrowsDomainException()
    {
        _providerRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Domain.Entities.ServiceProvider?)null);
        var existing = Domain.Entities.ServiceProvider.Create(
            "Other Co", "Person", "other@example.dk", "+4520123456", "Addr", "Aarhus", "12345678", "hash");
        _providerRepo.GetByCvrAsync("12345678", Arg.Any<CancellationToken>()).Returns(existing);

        var act = async () => await CreateHandler().Handle(ValidCommand(), default);
        await act.Should().ThrowAsync<DomainException>().WithMessage("*CVR*");
    }
}
