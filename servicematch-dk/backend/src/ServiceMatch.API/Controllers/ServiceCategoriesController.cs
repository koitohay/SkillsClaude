using MediatR;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/categories")]
public sealed class ServiceCategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
        => Ok(await sender.Send(new GetCategoriesQuery(), ct));
}
