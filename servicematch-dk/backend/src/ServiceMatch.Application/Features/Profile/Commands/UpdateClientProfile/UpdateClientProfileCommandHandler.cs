using MediatR;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Exceptions;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.Profile.Commands.UpdateClientProfile;

public sealed class UpdateClientProfileCommandHandler(
    IClientRepository clientRepo,
    IUnitOfWork uow)
    : IRequestHandler<UpdateClientProfileCommand, ClientDto>
{
    public async Task<ClientDto> Handle(UpdateClientProfileCommand request, CancellationToken ct)
    {
        var client = await clientRepo.GetByIdAsync(request.ClientId, ct)
            ?? throw new NotFoundException("Client not found.");

        client.Update(request.FullName, request.Email, request.PhoneNumber);
        await clientRepo.UpdateAsync(client, ct);
        await uow.SaveChangesAsync(ct);

        return new ClientDto(client.Id, client.FullName, client.Email.Value, client.PhoneNumber.Value);
    }
}
