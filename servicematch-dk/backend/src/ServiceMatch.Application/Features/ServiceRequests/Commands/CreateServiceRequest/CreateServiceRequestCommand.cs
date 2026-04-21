using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.ServiceRequests.Commands.CreateServiceRequest;

public record CreateServiceRequestCommand(
    Guid ClientId,
    int? CategoryId,
    string? FreeTextDescription,
    DateOnly RequestedDate,
    TimeOnly RequestedTime,
    string City) : IRequest<ServiceRequestDto>;
