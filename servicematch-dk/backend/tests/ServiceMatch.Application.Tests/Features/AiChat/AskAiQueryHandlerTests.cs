using FluentAssertions;
using NSubstitute;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.AiChat;

namespace ServiceMatch.Application.Tests.Features.AiChat;

public class AskAiQueryHandlerTests
{
    private readonly IAiChatService _chatService = Substitute.For<IAiChatService>();

    private AskAiQueryHandler CreateHandler() => new(_chatService);

    [Fact]
    public async Task Handle_DelegatesToChatService()
    {
        var messages = new List<ChatMessage> { new("user", "Hej!") };
        _chatService.ChatAsync(messages, Arg.Any<CancellationToken>()).Returns("Hej fra AI!");

        var result = await CreateHandler().Handle(new AskAiQuery(messages), default);

        result.Should().Be("Hej fra AI!");
        await _chatService.Received(1).ChatAsync(messages, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var messages = new List<ChatMessage>();
        _chatService.ChatAsync(Arg.Any<IReadOnlyList<ChatMessage>>(), cts.Token).Returns("ok");

        await CreateHandler().Handle(new AskAiQuery(messages), cts.Token);

        await _chatService.Received(1).ChatAsync(Arg.Any<IReadOnlyList<ChatMessage>>(), cts.Token);
    }
}
