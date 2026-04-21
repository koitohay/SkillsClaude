using MediatR;
using ServiceMatch.Domain.Interfaces;

namespace ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;

public interface IServiceCategoryRepository
{
    Task<IReadOnlyList<Domain.Entities.ServiceCategory>> GetAllAsync(CancellationToken ct = default);
}

public sealed class GetCategoriesQueryHandler(IServiceCategoryRepository categoryRepo)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await categoryRepo.GetAllAsync(ct);
        return categories.Select(c => new CategoryDto(c.Id, c.Name, c.Slug)).ToList();
    }
}
