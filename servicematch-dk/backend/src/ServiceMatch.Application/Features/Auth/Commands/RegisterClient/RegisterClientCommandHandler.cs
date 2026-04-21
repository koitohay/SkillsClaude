using MediatR;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Auth.Commands.RegisterClient;

public sealed class RegisterClientCommandHandler(
    IClientRepository clientRepo,
    IPasswordHasher passwordHasher,
    IUnitOfWork uow)
    : IRequestHandler<RegisterClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(RegisterClientCommand request, CancellationToken ct)
    {
        var existing = await clientRepo.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new DomainException($"Email '{request.Email}' is already registered.");

        var hash = passwordHasher.Hash(request.Password);
        var client = Domain.Entities.Client.Create(request.FullName, request.Email, request.PhoneNumber, hash);

        await clientRepo.AddAsync(client, ct);
        await uow.SaveChangesAsync(ct);

        return new ClientDto(client.Id, client.FullName, client.Email.Value, client.PhoneNumber.Value);
    }
}
