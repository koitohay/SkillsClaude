using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ServiceMatch.Application.Common.Interfaces;

namespace ServiceMatch.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
            return value is not null && Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string Role =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
