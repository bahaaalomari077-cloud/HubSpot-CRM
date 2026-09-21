using Crm.Domain.Enums;

namespace Crm.Application.Messaging.Dtos;

/// <summary>Request to send an outbound message on a channel.</summary>
public class SendMessageDto
{
    public Guid ContactId { get; set; }
    public MessageChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;  // email or E.164 phone
    public string? Subject { get; set; }
    public string Body { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
}

/// <summary>What the caller gets back after a send attempt.</summary>
public class SendMessageResultDto
{
    public Guid MessageId { get; set; }
    public MessageStatus Status { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? FailureReason { get; set; }
}
