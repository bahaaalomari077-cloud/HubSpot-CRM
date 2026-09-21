using Crm.Domain.Enums;

namespace Crm.Application.Messaging.Providers;

/// <summary>What a provider needs to dispatch one message.</summary>
public record SendRequest(
    string Recipient,     // email address or E.164 phone number
    string? Subject,      // used by email; ignored by WhatsApp
    string Body);

/// <summary>Outcome of a dispatch attempt — never throws for expected failures.</summary>
public record SendResult(
    bool Success,
    string? ProviderMessageId = null,
    string? FailureReason = null)
{
    public static SendResult Ok(string providerMessageId) => new(true, providerMessageId);
    public static SendResult Fail(string reason) => new(false, null, reason);
}

/// <summary>
/// A channel transport (SMTP, WhatsApp, …). One implementation per channel;
/// the factory picks the right one at send time.
/// </summary>
public interface IMessageProvider
{
    /// <summary>Which channel this provider serves.</summary>
    MessageChannel Channel { get; }

    Task<SendResult> SendAsync(SendRequest request, CancellationToken ct = default);
}
