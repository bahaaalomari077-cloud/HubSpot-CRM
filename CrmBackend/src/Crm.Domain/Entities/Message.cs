using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

/// <summary>
/// An immutable record of a communication that happened on some channel.
/// Append-only by intent — the audit trail, as opposed to Activity's work queue.
/// </summary>
public class Message : BaseEntity
{
    public MessageChannel Channel { get; set; } = MessageChannel.Email;
    public MessageDirection Direction { get; set; } = MessageDirection.Inbound;

    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsRead { get; set; }

    // --- outbound addressing ---
    /// <summary>Destination the message was sent to (email address or phone in E.164).</summary>
    public string? Recipient { get; set; }

    // --- delivery status tracking ---
    public MessageStatus Status { get; set; } = MessageStatus.Draft;

    /// <summary>Provider's own id for the message (SMTP message-id / WhatsApp wamid), for webhook correlation.</summary>
    public string? ProviderMessageId { get; set; }

    /// <summary>Reason a send failed, surfaced from the provider.</summary>
    public string? FailureReason { get; set; }

    public int AttemptCount { get; set; }
    public DateTimeOffset? LastAttemptAt { get; set; }

    // The contact this message is with.
    public Guid ContactId { get; set; }
    public Contact Contact { get; set; } = null!;

    // The user who sent/handled it (nullable for purely inbound machine messages).
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}
