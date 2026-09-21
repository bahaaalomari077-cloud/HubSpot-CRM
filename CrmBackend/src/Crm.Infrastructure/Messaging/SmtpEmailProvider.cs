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
        try
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
            msg.To.Add(MailboxAddress.Parse(request.Recipient));
            msg.Subject = request.Subject ?? "(no subject)";
            msg.Body = new TextPart("plain") { Text = request.Body };

            using var client = new SmtpClient();
            var secure = _opt.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync(_opt.Host, _opt.Port, secure, ct);

            if (!string.IsNullOrEmpty(_opt.Username))
                await client.AuthenticateAsync(_opt.Username, _opt.Password, ct);

            // Ensure a Message-Id exists so we can correlate later.
            msg.MessageId ??= MimeUtils.GenerateMessageId();

            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);

            return SendResult.Ok(msg.MessageId);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "SMTP send failed to {Recipient}", request.Recipient);
            return SendResult.Fail($"SMTP error: {ex.Message}");
        }
    }
}
