using MediatR;
using ServiceMatch.Application.Common.Interfaces;

namespace ServiceMatch.Application.Features.AiChat;

public sealed class AskAiQueryHandler(IAiChatService chatService)
    : IRequestHandler<AskAiQuery, string>
{
    public Task<string> Handle(AskAiQuery request, CancellationToken ct)
        => chatService.ChatAsync(request.Messages, ct);
}
