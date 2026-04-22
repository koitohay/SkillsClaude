using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.AiChat;

namespace ServiceMatch.API.Controllers;

[ApiController]
[AllowAnonymous]
public class ChatController(IMediator mediator) : ControllerBase
{
    [HttpPost("/api/v1/chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest body, CancellationToken ct)
    {
        if (body.Messages is null || body.Messages.Count == 0)
            return BadRequest(new { error = "Messages cannot be empty." });

        var messages = body.Messages
            .Select(m => new ChatMessage(m.Role, m.Content))
            .ToList();

        var reply = await mediator.Send(new AskAiQuery(messages), ct);
        return Ok(new { reply });
    }
}

public record ChatRequest(IReadOnlyList<ChatMessageDto> Messages);
public record ChatMessageDto(string Role, string Content);
