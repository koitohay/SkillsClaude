using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Features.Profile.Commands.AddProviderService;
using ServiceMatch.Application.Features.Profile.Commands.DeleteProviderService;
using ServiceMatch.Application.Features.Profile.Commands.UpdateClientProfile;
using ServiceMatch.Application.Features.Profile.Commands.UpdateProviderProfile;
using ServiceMatch.Application.Features.Profile.Commands.UpdateProviderService;
using ServiceMatch.Application.Features.Profile.Queries.GetMyProfile;

namespace ServiceMatch.API.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public sealed class ProfileController(ISender sender) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID not found."));

    private string CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
        => Ok(await sender.Send(new GetMyProfileQuery(CurrentUserId, CurrentRole), ct));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest body, CancellationToken ct)
    {
        if (User.IsInRole("Client"))
        {
            if (string.IsNullOrWhiteSpace(body.FullName) ||
                string.IsNullOrWhiteSpace(body.Email) ||
                string.IsNullOrWhiteSpace(body.PhoneNumber))
                return BadRequest(new { error = "Required fields are missing." });

            var result = await sender.Send(new UpdateClientProfileCommand(
                CurrentUserId, body.FullName, body.Email, body.PhoneNumber), ct);
            return Ok(result);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(body.CompanyName) ||
                string.IsNullOrWhiteSpace(body.ContactName) ||
                string.IsNullOrWhiteSpace(body.Email) ||
                string.IsNullOrWhiteSpace(body.PhoneNumber) ||
                string.IsNullOrWhiteSpace(body.Address) ||
                string.IsNullOrWhiteSpace(body.City))
                return BadRequest(new { error = "Required fields are missing." });

            var result = await sender.Send(new UpdateProviderProfileCommand(
                CurrentUserId, body.CompanyName, body.ContactName, body.Email,
                body.PhoneNumber, body.Address, body.City), ct);
            return Ok(result);
        }
    }

    [HttpPost("services")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> AddService([FromBody] ServiceBody body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Description))
            return BadRequest(new { error = "Required fields are missing." });

        var result = await sender.Send(new AddProviderServiceCommand(
            CurrentUserId, body.CategoryId, body.Name, body.Description, body.BasePrice), ct);
        return CreatedAtAction(nameof(GetProfile), result);
    }

    [HttpPut("services/{serviceId:guid}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> UpdateService(Guid serviceId, [FromBody] ServiceBody body, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Description))
            return BadRequest(new { error = "Required fields are missing." });

        var result = await sender.Send(new UpdateProviderServiceCommand(
            CurrentUserId, serviceId, body.Name, body.Description, body.BasePrice), ct);
        return Ok(result);
    }

    [HttpDelete("services/{serviceId:guid}")]
    [Authorize(Roles = "Provider")]
    public async Task<IActionResult> DeleteService(Guid serviceId, CancellationToken ct)
    {
        await sender.Send(new DeleteProviderServiceCommand(CurrentUserId, serviceId), ct);
        return NoContent();
    }
}

public record UpdateProfileRequest(
    string? FullName,
    string? CompanyName,
    string? ContactName,
    string? Email,
    string? PhoneNumber,
    string? Address,
    string? City);

public record ServiceBody(
    int? CategoryId,
    string? Name,
    string? Description,
    decimal BasePrice);
