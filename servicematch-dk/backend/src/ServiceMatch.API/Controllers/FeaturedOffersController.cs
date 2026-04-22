using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Infrastructure.BackgroundServices;

namespace ServiceMatch.API.Controllers;

[ApiController]
[AllowAnonymous]
public class FeaturedOffersController(IMemoryCache cache) : ControllerBase
{
    /// <summary>
    /// Returns the homepage featured offers curated by the OffersFeedAgent.
    /// Public endpoint — no authentication required.
    /// Returns an empty array while the agent is warming up on first start.
    /// </summary>
    [HttpGet("/api/v1/featured-offers")]
    public IActionResult Get()
    {
        var offers = cache.Get<List<FeaturedOfferDto>>(OffersFeedAgentService.CacheKey) ?? [];
        return Ok(offers);
    }
}
