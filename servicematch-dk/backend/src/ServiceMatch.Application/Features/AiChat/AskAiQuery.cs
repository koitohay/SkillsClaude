using MediatR;
using ServiceMatch.Application.Common.Interfaces;

namespace ServiceMatch.Application.Features.AiChat;

public record AskAiQuery(IReadOnlyList<ChatMessage> Messages) : IRequest<string>;
