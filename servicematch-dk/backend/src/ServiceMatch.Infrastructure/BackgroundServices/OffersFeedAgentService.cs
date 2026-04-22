using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceMatch.Application.DTOs;
using ServiceMatch.Domain.Enums;
using ServiceMatch.Infrastructure.Options;
using ServiceMatch.Infrastructure.Persistence;

namespace ServiceMatch.Infrastructure.BackgroundServices;

/// <summary>
/// Background agent that periodically asks Claude (via the internal Novo Nordisk
/// AI marketplace — same OpenAI-compatible endpoint used by AnthropicChatService)
/// to curate the best open offers for the homepage feed.
///
/// Cycle interval is controlled by Anthropic:FeedRefreshSeconds in appsettings.json
/// (default: 300 seconds / 5 minutes).
///
/// The agent is given two tools:
///   get_open_offers  — reads live data from the database
///   set_featured_offers — writes Claude's selection to IMemoryCache
///
/// The cache key "featured-offers" is read by FeaturedOffersController.
/// </summary>
public sealed class OffersFeedAgentService(
    IServiceScopeFactory scopeFactory,
    IMemoryCache cache,
    IOptions<AnthropicOptions> options,
    IHttpClientFactory httpFactory,
    ILogger<OffersFeedAgentService> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public const string CacheKey = "featured-offers";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the app to fully start before first run
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunAgentCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OffersFeedAgent: cycle failed — will retry next interval");
            }

            var intervalSeconds = options.Value.FeedRefreshSeconds;
            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }

    // ── Agent loop ────────────────────────────────────────────────────────────

    private async Task RunAgentCycleAsync(CancellationToken ct)
    {
        logger.LogInformation("OffersFeedAgent: starting curation cycle");

        var curationWritten = false;

        var messages = new List<object>
        {
            new
            {
                role = "system",
                content =
                    "You are the curation agent for ServiceMatch DK, a Danish service marketplace. " +
                    "You MUST always complete your task by calling set_featured_offers. " +
                    "STEP 1: Call get_open_offers to retrieve the current offers. " +
                    "STEP 2: Choose up to 6 offers using these criteria (in priority order): " +
                    "  a) Geographic diversity — pick offers from different Danish cities. " +
                    "  b) Category diversity — no more than 2 offers from the same category. " +
                    "  c) Competitive price — prefer lower prices per category/city. " +
                    "  d) Recency — prefer newer offers. " +
                    "STEP 3: You MUST call set_featured_offers with your selections. " +
                    "Do NOT respond with text. Do NOT skip step 3. " +
                    "If there are no offers, call set_featured_offers with an empty selections array."
            },
            new
            {
                role = "user",
                content = "Execute the curation cycle now. Call get_open_offers, then call set_featured_offers."
            }
        };

        var tools = BuildToolDefinitions();
        var http = httpFactory.CreateClient("anthropic");

        // Force tool use on every call — the model must not reply with plain text
        var toolChoice = new { type = "required" };

        for (var iteration = 0; iteration < 5; iteration++)
        {
            var requestBody = new
            {
                model = options.Value.Model,
                max_tokens = 2048,
                tools,
                tool_choice = toolChoice,
                messages
            };
            var json = JsonSerializer.Serialize(requestBody, JsonOpts);

            using var req = new HttpRequestMessage(HttpMethod.Post, options.Value.ApiUrl);
            req.Headers.Add("Authorization", $"Bearer {options.Value.ApiKey}");
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await http.SendAsync(req, ct);
            var responseJson = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                logger.LogError("OffersFeedAgent: API error {Status}: {Body}", resp.StatusCode, responseJson);
                break;
            }

            var doc = JsonNode.Parse(responseJson)!;
            var choice = doc["choices"]![0]!;
            var finishReason = choice["finish_reason"]?.GetValue<string>();
            var message = choice["message"]!;

            messages.Add(JsonSerializer.Deserialize<object>(message.ToJsonString(), JsonOpts)!);

            if (finishReason == "stop" || finishReason == "end_turn")
                break;

            if (finishReason != "tool_calls") break;

            var toolCalls = message["tool_calls"]?.AsArray();
            if (toolCalls is null || toolCalls.Count == 0) break;

            var toolResults = new List<object>();
            foreach (var toolCall in toolCalls)
            {
                var toolId   = toolCall!["id"]!.GetValue<string>();
                var toolName = toolCall["function"]!["name"]!.GetValue<string>();
                var argsJson = toolCall["function"]!["arguments"]!.GetValue<string>();
                var input    = JsonNode.Parse(argsJson)?.AsObject() ?? [];

                var result = await ExecuteToolAsync(toolName, input, ct);
                toolResults.Add(new { role = "tool", tool_call_id = toolId, content = result });

                if (toolName == "set_featured_offers")
                    curationWritten = true;
            }

            messages.AddRange(toolResults);

            if (curationWritten) break;
        }

        // Fallback: if the AI never called set_featured_offers, select top offers by price
        if (!curationWritten)
        {
            logger.LogWarning("OffersFeedAgent: AI did not call set_featured_offers — applying rules-based fallback");
            await ApplyFallbackCurationAsync(ct);
        }
        else
        {
            logger.LogInformation("OffersFeedAgent: cycle complete");
        }
    }

    private async Task ApplyFallbackCurationAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Load with EF-managed conversions, then sort + project to strings in memory.
        var rows = await (
            from o in db.Offers
            join r in db.ServiceRequests on o.ServiceRequestId equals r.Id
            join c in db.ServiceCategories on r.CategoryId equals c.Id into cats
            from cat in cats.DefaultIfEmpty()
            where o.Status == OfferStatus.Pending
               && (r.Status == ServiceRequestStatus.Open || r.Status == ServiceRequestStatus.OfferReceived)
            select new
            {
                o.Id, o.ServiceRequestId, o.Price, o.Message, o.CreatedAt,
                CategoryName = cat != null ? cat.Name : "General",
                r.City
            }
        ).Take(50).ToListAsync(ct);

        // Pick up to 6 — one per category/city combination, cheapest first
        var featured = new List<FeaturedOfferDto>();
        var seenCombos = new HashSet<string>();
        foreach (var row in rows.OrderBy(r => r.Price.Amount))
        {
            var combo = $"{row.CategoryName}:{row.City.Name}";
            if (!seenCombos.Add(combo)) continue;
            featured.Add(new FeaturedOfferDto(
                row.Id, row.ServiceRequestId, row.CategoryName, row.City.Name,
                row.Price.Amount, row.Message,
                "Competitively priced offer in this category and city.",
                row.CreatedAt));
            if (featured.Count >= 6) break;
        }

        cache.Set(CacheKey, featured, TimeSpan.FromSeconds(options.Value.FeedRefreshSeconds * 2));
        logger.LogInformation("OffersFeedAgent: fallback cached {Count} featured offers", featured.Count);
    }

    // ── Tool dispatcher ───────────────────────────────────────────────────────

    private async Task<string> ExecuteToolAsync(string toolName, JsonObject input, CancellationToken ct) =>
        toolName switch
        {
            "get_open_offers"     => await GetOpenOffersAsync(ct),
            "set_featured_offers" => await SetFeaturedOffersAsync(input, ct),
            _                     => $"Unknown tool: {toolName}"
        };

    private async Task<string> GetOpenOffersAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Load offers with City as DanishCity (EF handles the conversion),
        // then project to strings in memory after materialisation.
        var raw = await (
            from o in db.Offers
            join r in db.ServiceRequests on o.ServiceRequestId equals r.Id
            join c in db.ServiceCategories on r.CategoryId equals c.Id into cats
            from cat in cats.DefaultIfEmpty()
            where o.Status == OfferStatus.Pending
               && (r.Status == ServiceRequestStatus.Open || r.Status == ServiceRequestStatus.OfferReceived)
            orderby o.CreatedAt descending
            select new
            {
                o.Id,
                o.ServiceRequestId,
                o.Price,
                o.Message,
                o.CreatedAt,
                CategoryName = cat != null ? cat.Name : "General",
                r.City
            }
        ).Take(50).ToListAsync(ct);

        if (raw.Count == 0)
            return "No open offers found in the database.";

        var sb = new StringBuilder();
        sb.AppendLine($"Found {raw.Count} open offers:");
        foreach (var o in raw)
        {
            var age = (DateTimeOffset.UtcNow - o.CreatedAt).TotalHours;
            sb.AppendLine(
                $"- offer_id:{o.Id} | category:{o.CategoryName} | city:{o.City.Name} | " +
                $"price:{o.Price.Amount:0} DKK | message:{o.Message ?? "(none)"} | " +
                $"age:{age:0.0}h");
        }

        return sb.ToString().TrimEnd();
    }

    private async Task<string> SetFeaturedOffersAsync(JsonObject input, CancellationToken ct)
    {
        if (!input.TryGetPropertyValue("selections", out var selectionsNode)
            || selectionsNode is not JsonArray selArray)
            return "Error: selections array is required.";

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var featured = new List<FeaturedOfferDto>();
        foreach (var item in selArray)
        {
            var obj = item?.AsObject();
            if (obj is null) continue;

            if (!obj.TryGetPropertyValue("offer_id", out var idNode)
                || !Guid.TryParse(idNode?.GetValue<string>(), out var offerId))
                continue;

            var reason = obj.TryGetPropertyValue("reason", out var rNode)
                ? rNode?.GetValue<string>() ?? ""
                : "";

            var row = await (
                from o in db.Offers
                join r in db.ServiceRequests on o.ServiceRequestId equals r.Id
                join c in db.ServiceCategories on r.CategoryId equals c.Id into cats
                from cat in cats.DefaultIfEmpty()
                where o.Id == offerId
                select new
                {
                    o.Id,
                    o.ServiceRequestId,
                    o.Price,
                    o.Message,
                    o.CreatedAt,
                    CategoryName = cat != null ? cat.Name : "General",
                    r.City
                }
            ).FirstOrDefaultAsync(ct);

            if (row is null) continue;

            featured.Add(new FeaturedOfferDto(
                row.Id,
                row.ServiceRequestId,
                row.CategoryName,
                row.City.Name,
                row.Price.Amount,
                row.Message,
                reason,
                row.CreatedAt));
        }

        cache.Set(CacheKey, featured, TimeSpan.FromSeconds(options.Value.FeedRefreshSeconds * 2));

        logger.LogInformation("OffersFeedAgent: cached {Count} featured offers", featured.Count);
        return $"Successfully featured {featured.Count} offers.";
    }

    // ── Tool definitions (OpenAI function-calling format) ─────────────────────

    private static object[] BuildToolDefinitions() =>
    [
        new
        {
            type = "function",
            function = new
            {
                name = "get_open_offers",
                description =
                    "Retrieves all currently open (Pending) offers on the platform with their price, " +
                    "category, city, and age. Call this first to see what is available.",
                parameters = new
                {
                    type = "object",
                    properties = new { },
                    required = Array.Empty<string>()
                }
            }
        },
        new
        {
            type = "function",
            function = new
            {
                name = "set_featured_offers",
                description =
                    "Publishes the curated selection of offers to the homepage feed. " +
                    "Call this once with your final selection.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        selections = new
                        {
                            type = "array",
                            description = "Array of selected offers with a curation reason for each",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    offer_id = new { type = "string", description = "UUID of the offer to feature" },
                                    reason   = new { type = "string", description = "One sentence explaining why this offer was selected" }
                                },
                                required = new[] { "offer_id", "reason" }
                            }
                        }
                    },
                    required = new[] { "selections" }
                }
            }
        }
    ];
}
