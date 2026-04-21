using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.Auth.Commands.RegisterClient;
using ServiceMatch.Domain.Entities;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Tests.Features.Auth;

public class RegisterClientCommandHandlerTests
{
    private readonly IClientRepository _clientRepo = Substitute.For<IClientRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private RegisterClientCommandHandler CreateHandler() =>
        new(_clientRepo, _passwordHasher, _uow);

    [Fact]
    public async Task Handle_WithValidInput_CreatesClientAndReturnsDto()
    {
        _clientRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Client?)null);
        _passwordHasher.Hash("Password1!").Returns("hashed");

        var command = new RegisterClientCommand("Jane Doe", "jane@example.com", "+4520123456", "Password1!");
        var result = await CreateHandler().Handle(command, default);

        result.Email.Should().Be("jane@example.com");
        result.FullName.Should().Be("Jane Doe");
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ThrowsDomainException()
    {
        var existing = Client.Create("Existing", "jane@example.com", "+4520123456", "hash");
        _clientRepo.GetByEmailAsync("jane@example.com", Arg.Any<CancellationToken>())
            .Returns(existing);

        var command = new RegisterClientCommand("Jane Doe", "jane@example.com", "+4520123456", "Password1!");
        var act = async () => await CreateHandler().Handle(command, default);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*already registered*");
    }

    [Fact]
    public async Task Handle_WithInvalidPhone_ThrowsInvalidPhoneNumberException()
    {
        _clientRepo.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Client?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed");

        var command = new RegisterClientCommand("Jane Doe", "jane@example.com", "invalid-phone", "Password1!");
        var act = async () => await CreateHandler().Handle(command, default);

        await act.Should().ThrowAsync<InvalidPhoneNumberException>();
    }
}
