using MediatR;

namespace ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;

public record CategoryDto(int Id, string Name, string Slug);

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
