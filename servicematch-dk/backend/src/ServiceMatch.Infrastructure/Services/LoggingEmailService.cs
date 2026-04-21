using Microsoft.Extensions.Logging;
using ServiceMatch.Application.Common.Interfaces;

namespace ServiceMatch.Infrastructure.Services;

public sealed class LoggingEmailService(ILogger<LoggingEmailService> logger) : IEmailService
{
    public Task SendConfirmationAsync(string toEmail, string toName, string subject, string body, CancellationToken ct)
    {
        logger.LogInformation(
            "[EMAIL] To: {Email} ({Name}) | Subject: {Subject} | Body: {Body}",
            toEmail, toName, subject, body);
        return Task.CompletedTask;
    }
}
