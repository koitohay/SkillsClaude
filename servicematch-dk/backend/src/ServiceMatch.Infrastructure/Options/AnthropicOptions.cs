namespace ServiceMatch.Infrastructure.Options;

public class AnthropicOptions
{
    public string ApiKey { get; set; } = "";
    public string ApiUrl { get; set; } = "https://api.marketplace.novo-genai.com/v1/chat/completions";
    public string Model { get; set; } = "anthropic_claude_haiku_4_5_v1_0";
    /// <summary>How often the OffersFeedAgentService refreshes the homepage feed (seconds).</summary>
    public int FeedRefreshSeconds { get; set; } = 300;
}
