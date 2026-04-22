namespace ServiceMatch.Application.Common.Interfaces;

public interface IAiChatService
{
    Task<string> ChatAsync(IReadOnlyList<ChatMessage> history, CancellationToken ct);
}

public record ChatMessage(string Role, string Content);
