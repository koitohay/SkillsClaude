using MediatR;
using ServiceMatch.Application.DTOs;

namespace ServiceMatch.Application.Features.Providers.Queries.GetProvidersByCategory;

public record GetProvidersByCategoryQuery(int? CategoryId, string? SearchTerm = null) : IRequest<IReadOnlyList<ProviderWithServicesDto>>;
