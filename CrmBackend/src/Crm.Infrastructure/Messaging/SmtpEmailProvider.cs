using Crm.Application.Messaging.Providers;
using Crm.Domain.Enums;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace Crm.Infrastructure.Messaging;

/// <summary>Sends email over SMTP via MailKit.</summary>
public class SmtpEmailProvider : IMessageProvider
{
    private readonly SmtpOptions _opt;
    private readonly ILogger<SmtpEmailProvider> _log;

    public SmtpEmailProvider(IOptions<SmtpOptions> opt, ILogger<SmtpEmailProvider> log)
    {
        _opt = opt.Value;
        _log = log;
    }

    public MessageChannel Channel => MessageChannel.Email;

    public async Task<SendResult> SendAsync(SendRequest request, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
        msg.To.Add(MailboxAddress.Parse(request.Recipient));
        msg.Subject = request.Subject ?? "(no subject)";
        msg.Body = new TextPart("plain") { Text = request.Body };

        // Ensure a Message-Id exists so we can correlate later.
        msg.MessageId ??= MimeUtils.GenerateMessageId();

        // Some networks drop SMTP sessions intermittently, so a connection that
        // stalls is abandoned quickly and retried rather than blocking the caller
        // for the full socket timeout.
        Exception? last = null;
        for (var attempt = 1; attempt <= _opt.MaxAttempts; attempt++)
        {
            using var attemptCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            attemptCts.CancelAfter(TimeSpan.FromSeconds(_opt.AttemptTimeoutSeconds));

            try
            {
                using var client = new SmtpClient { Timeout = _opt.AttemptTimeoutSeconds * 1000 };
                var secure = _opt.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;

                await client.ConnectAsync(_opt.Host, _opt.Port, secure, attemptCts.Token);

                if (!string.IsNullOrEmpty(_opt.Username))
                    await client.AuthenticateAsync(_opt.Username, _opt.Password, attemptCts.Token);

                await client.SendAsync(msg, attemptCts.Token);
                await client.DisconnectAsync(true, attemptCts.Token);

                if (attempt > 1)
                    _log.LogInformation("SMTP send to {Recipient} succeeded on attempt {Attempt}",
                        request.Recipient, attempt);

                return SendResult.Ok(msg.MessageId);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                // Bad credentials will never succeed — fail immediately.
                _log.LogWarning(ex, "SMTP authentication rejected for {Username}", _opt.Username);
                return SendResult.Fail($"SMTP authentication failed: {ex.Message}");
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw; // the caller gave up
            }
            catch (Exception ex)
            {
                last = ex;
                _log.LogDebug("SMTP attempt {Attempt}/{Max} to {Recipient} failed: {Error}",
                    attempt, _opt.MaxAttempts, request.Recipient, ex.Message);
            }
        }

        _log.LogWarning(last, "SMTP send to {Recipient} failed after {Max} attempts",
            request.Recipient, _opt.MaxAttempts);
        return SendResult.Fail(
            $"SMTP error after {_opt.MaxAttempts} attempts: {last?.Message ?? "unknown"}");
    }
}
