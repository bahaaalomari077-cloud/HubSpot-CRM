using System.ComponentModel.DataAnnotations;
using Crm.Application.Common.Validation;
using Crm.Domain.Enums;

namespace Crm.Application.Messaging.Dtos;

/// <summary>Request to send an outbound message on a channel.</summary>
public class SendMessageDto : IValidatableObject
{
    [NotEmptyGuid(ErrorMessage = "ContactId is required.")]
    public Guid ContactId { get; set; }

    [EnumDataType(typeof(MessageChannel), ErrorMessage = "Unknown channel.")]
    public MessageChannel Channel { get; set; }

    [Required(ErrorMessage = "Recipient is required.")]
    [StringLength(320)]
    public string Recipient { get; set; } = string.Empty;  // email or E.164 phone

    [StringLength(256)]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Message body is required.")]
    [StringLength(10000, MinimumLength = 1)]
    public string Body { get; set; } = string.Empty;

    public Guid? UserId { get; set; }

    /// <summary>Recipient format depends on the channel, so it's checked here.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext _)
    {
        var recipient = Recipient?.Trim() ?? string.Empty;

        if (Channel == MessageChannel.Email &&
            !new EmailAddressAttribute().IsValid(recipient))
        {
            yield return new ValidationResult(
                "Recipient must be a valid email address for the Email channel.",
                new[] { nameof(Recipient) });
        }

        if (Channel is MessageChannel.WhatsApp or MessageChannel.Sms &&
            !System.Text.RegularExpressions.Regex.IsMatch(recipient, @"^\+?[0-9]{7,15}$"))
        {
            yield return new ValidationResult(
                "Recipient must be a phone number (7-15 digits) for this channel.",
                new[] { nameof(Recipient) });
        }
    }
}

/// <summary>What the caller gets back after a send attempt.</summary>
public class SendMessageResultDto
{
    public Guid MessageId { get; set; }
    public MessageStatus Status { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? FailureReason { get; set; }
}
