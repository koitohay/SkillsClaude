namespace ServiceMatch.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendConfirmationAsync(string toEmail, string toName, string subject, string body, CancellationToken ct = default);
}
