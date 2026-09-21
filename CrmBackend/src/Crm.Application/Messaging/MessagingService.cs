using Crm.Application.Common.Interfaces;
using Crm.Application.Messaging.Dtos;
using Crm.Application.Messaging.Providers;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Messaging;

public class MessagingService : IMessagingService
{
    private readonly ICrmDbContext _db;
    private readonly IMessageProviderFactory _providers;
    private readonly ILogger<MessagingService> _log;

    public MessagingService(
        ICrmDbContext db,
        IMessageProviderFactory providers,
        ILogger<MessagingService> log)
    {
        _db = db;
        _providers = providers;
        _log = log;
    }

    public async Task<SendMessageResultDto> SendAsync(SendMessageDto dto, CancellationToken ct = default)
    {
        // 1. Persist FIRST, as Queued. If the process dies mid-send, the record
        //    survives and a background sweep can retry it — no message is ever lost.
        var message = new Message
        {
            ContactId = dto.ContactId,
            UserId = dto.UserId,
            Channel = dto.Channel,
            Direction = MessageDirection.Outbound,
            Subject = dto.Subject,
            Body = dto.Body,
            Recipient = dto.Recipient,
            Status = MessageStatus.Queued,
            SentAt = DateTimeOffset.UtcNow
        };
        _db.Messages.Add(message);
        await _db.SaveChangesAsync(ct);

        // 2. Guard: is there a provider for this channel?
        if (!_providers.Supports(dto.Channel))
        {
            return await FailAsync(message,
                $"No provider configured for channel '{dto.Channel}'.", ct);
        }

        // 3. Dispatch.
        message.AttemptCount++;
        message.LastAttemptAt = DateTimeOffset.UtcNow;

        SendResult result;
        try
        {
            var provider = _providers.For(dto.Channel);
            result = await provider.SendAsync(
                new SendRequest(dto.Recipient, dto.Subject, dto.Body), ct);
        }
        catch (Exception ex)
        {
            // Unexpected transport error — treat as failure, keep the record.
            _log.LogError(ex, "Provider threw while sending message {MessageId}", message.Id);
            return await FailAsync(message, ex.Message, ct);
        }

        // 4. Record the outcome.
        if (result.Success)
        {
            message.Status = MessageStatus.Sent;
            message.ProviderMessageId = result.ProviderMessageId;
            message.FailureReason = null;
        }
        else
        {
            message.Status = MessageStatus.Failed;
            message.FailureReason = result.FailureReason;
        }
        await _db.SaveChangesAsync(ct);

        return ToResult(message);
    }

    private async Task<SendMessageResultDto> FailAsync(Message message, string reason, CancellationToken ct)
    {
        message.Status = MessageStatus.Failed;
        message.FailureReason = reason;
        await _db.SaveChangesAsync(ct);
        return ToResult(message);
    }

    private static SendMessageResultDto ToResult(Message m) => new()
    {
        MessageId = m.Id,
        Status = m.Status,
        ProviderMessageId = m.ProviderMessageId,
        FailureReason = m.FailureReason
    };
}
