using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceMatch.Application.Common.Interfaces;
using ServiceMatch.Infrastructure.Options;

namespace ServiceMatch.Infrastructure.Services;

public sealed class AcsEmailService(
    IOptions<AcsOptions> options,
    ILogger<AcsEmailService> logger) : IEmailService
{
    public async Task SendConfirmationAsync(string toEmail, string toName, string subject, string body, CancellationToken ct = default)
    {
        var client = new EmailClient(options.Value.ConnectionString);

        var message = new EmailMessage(
            senderAddress: options.Value.SenderAddress,
            recipients: new EmailRecipients([new EmailAddress(toEmail, toName)]),
            content: new EmailContent(subject) { PlainText = body });

        var operation = await client.SendAsync(WaitUntil.Started, message, ct);
        logger.LogInformation("ACS email enqueued to {Email}, operationId: {OperationId}", toEmail, operation.Id);
    }
}
