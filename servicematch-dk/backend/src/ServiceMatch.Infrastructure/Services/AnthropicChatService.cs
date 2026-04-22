using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Application.Features.ServiceCategories.Queries.GetCategories;
using ServiceMatch.Domain.Interfaces;
using ServiceMatch.Infrastructure.Options;

namespace ServiceMatch.Infrastructure.Services;

public sealed class AnthropicChatService(
    HttpClient http,
    IServiceCategoryRepository categoryRepo,
    IServiceProviderRepository providerRepo,
    IOptions<AnthropicOptions> options) : IAiChatService
{
    private const string ApiUrl = "https://api.marketplace.novo-genai.com/v1/chat/completions";
    private const string Model = "anthropic_claude_haiku_4_5_v1_0";
    private const string SystemPrompt =
        "You are a helpful assistant for ServiceMatch DK, a Danish online marketplace " +
        "where clients book home services (cleaning, plumbing, gardening, etc.) from local service providers. " +
        "Help visitors understand available services, find suitable providers, and guide them through placing a request. " +
        "Always respond in the same language the user writes in (Danish or English). " +
        "Use the available tools to look up live data — never invent provider names, prices, or phone numbers. " +
        "Keep replies concise and friendly.";

    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public async Task<string> ChatAsync(IReadOnlyList<ChatMessage> history, CancellationToken ct)
    {
        // OpenAI-compatible format: system prompt as first message
        var messages = new List<object>
        {
            new { role = "system", content = SystemPrompt }
        };
        foreach (var m in history)
            messages.Add(new { role = m.Role, content = (object)m.Content });

        var tools = BuildToolDefinitions();

        for (var iteration = 0; iteration < 10; iteration++)
        {
            var requestBody = new
            {
                model = Model,
                max_tokens = 1024,
                tools,
                messages
            };

            var json = JsonSerializer.Serialize(requestBody, JsonOpts);
            using var req = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            req.Headers.Add("Authorization", $"Bearer {options.Value.ApiKey}");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await http.SendAsync(req, ct);
            var responseJson = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Anthropic API error {resp.StatusCode}: {responseJson}");

            var doc = JsonNode.Parse(responseJson)!;
            var choice = doc["choices"]![0]!;
            var finishReason = choice["finish_reason"]?.GetValue<string>();
            var message = choice["message"]!;

            // Add assistant message to history
            messages.Add(JsonSerializer.Deserialize<object>(message.ToJsonString(), JsonOpts)!);

            if (finishReason == "stop" || finishReason == "end_turn")
            {
                return message["content"]?.GetValue<string>()
                    ?? "Beklager, jeg kunne ikke generere et svar.";
            }

            if (finishReason != "tool_calls")
                break;

            // Execute all tool calls and collect results
            var toolCalls = message["tool_calls"]?.AsArray();
            if (toolCalls is null || toolCalls.Count == 0) break;

            foreach (var toolCall in toolCalls)
            {
                var toolId = toolCall!["id"]!.GetValue<string>();
                var toolName = toolCall["function"]!["name"]!.GetValue<string>();
                var argsJson = toolCall["function"]!["arguments"]!.GetValue<string>();
                var input = JsonNode.Parse(argsJson)?.AsObject() ?? [];

                var result = await ExecuteToolAsync(toolName, input, ct);
                messages.Add(new { role = "tool", tool_call_id = toolId, content = result });
            }
        }

        return "Beklager, der opstod et problem med at behandle din forespørgsel.";
    }

    private async Task<string> ExecuteToolAsync(string toolName, JsonObject input, CancellationToken ct)
    {
        return toolName switch
        {
            "list_categories" => await ListCategoriesAsync(ct),
            "search_providers" => await SearchProvidersAsync(input, ct),
            "get_provider_details" => await GetProviderDetailsAsync(input, ct),
            _ => "Ukendt værktøj."
        };
    }

    private async Task<string> ListCategoriesAsync(CancellationToken ct)
    {
        var categories = await categoryRepo.GetAllAsync(ct);
        if (!categories.Any()) return "Ingen kategorier fundet.";
        return string.Join("\n", categories.Select(c => $"- {c.Name} (id: {c.Id})"));
    }

    private async Task<string> SearchProvidersAsync(JsonObject input, CancellationToken ct)
    {
        int? categoryId = null;
        string? city = null;

        if (input.TryGetPropertyValue("category_id", out var catNode) && catNode is not null)
            categoryId = catNode.GetValue<int>();
        if (input.TryGetPropertyValue("city", out var cityNode) && cityNode is not null)
            city = cityNode.GetValue<string>();

        var providers = await providerRepo.GetByCategoryAsync(categoryId, ct);

        if (!string.IsNullOrWhiteSpace(city))
            providers = providers.Where(p => p.City.Name.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!providers.Any()) return "Ingen udbydere fundet med de angivne kriterier.";

        var sb = new StringBuilder();
        foreach (var p in providers.Take(10))
        {
            sb.AppendLine($"**{p.CompanyName}** (id: {p.Id})");
            sb.AppendLine($"  By: {p.City.Name} | Tlf: {p.PhoneNumber.Value}");
            var minPrice = p.Services.Any() ? p.Services.Min(s => s.BasePrice.Amount) : 0;
            if (minPrice > 0) sb.AppendLine($"  Fra {minPrice:0} DKK");
        }
        return sb.ToString().TrimEnd();
    }

    private async Task<string> GetProviderDetailsAsync(JsonObject input, CancellationToken ct)
    {
        if (!input.TryGetPropertyValue("provider_id", out var idNode) || idNode is null)
            return "provider_id er påkrævet.";

        if (!Guid.TryParse(idNode.GetValue<string>(), out var providerId))
            return "Ugyldigt provider_id format.";

        var provider = await providerRepo.GetByIdAsync(providerId, ct);
        if (provider is null) return "Udbyder ikke fundet.";

        var sb = new StringBuilder();
        sb.AppendLine($"**{provider.CompanyName}**");
        sb.AppendLine($"Kontakt: {provider.ContactName}");
        sb.AppendLine($"E-mail: {provider.Email.Value}");
        sb.AppendLine($"Telefon: {provider.PhoneNumber.Value}");
        sb.AppendLine($"Adresse: {provider.Address}, {provider.City.Name}");
        sb.AppendLine($"CVR: {provider.CvrNumber.Value}");

        if (provider.Services.Any())
        {
            sb.AppendLine("\nYdelser:");
            foreach (var svc in provider.Services)
                sb.AppendLine($"  - {svc.Name}: {svc.BasePrice.Amount:0} DKK — {svc.Description}");
        }

        return sb.ToString().TrimEnd();
    }

    private static object[] BuildToolDefinitions() =>
    [
        new
        {
            type = "function",
            function = new
            {
                name = "list_categories",
                description = "List all available service categories on the platform.",
                parameters = new { type = "object", properties = new { }, required = Array.Empty<string>() }
            }
        },
        new
        {
            type = "function",
            function = new
            {
                name = "search_providers",
                description = "Search for service providers, optionally filtered by category and/or city.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        category_id = new { type = "integer", description = "Filter by category ID (optional)" },
                        city = new { type = "string", description = "Filter by city name, e.g. 'København' (optional)" }
                    },
                    required = Array.Empty<string>()
                }
            }
        },
        new
        {
            type = "function",
            function = new
            {
                name = "get_provider_details",
                description = "Get full details for a specific provider including all services, prices, address, and phone number.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        provider_id = new { type = "string", description = "The UUID of the provider" }
                    },
                    required = new[] { "provider_id" }
                }
            }
        }
    ];
}
