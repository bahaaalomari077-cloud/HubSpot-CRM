using System.ComponentModel.DataAnnotations;
using Crm.Domain.Enums;

namespace Crm.Application.Contacts.Dtos;

public class ContactDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }

    /// <summary>potential | contacted | interested | applied | client | rejected</summary>
    public ContactStatus Status { get; set; }

    /// <summary>What to do next with this contact.</summary>
    public string? NextAction { get; set; }
    public DateTimeOffset? NextActionDate { get; set; }

    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateContactDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(120, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>May be empty for company-style contacts entered as one word.</summary>
    [StringLength(120)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(256)]
    public string? Email { get; set; }

    // E.164-ish: optional +, 7–15 digits. Spaces/dashes stripped by the client.
    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "Phone must be 7-15 digits, optionally starting with '+'.")]
    public string? Phone { get; set; }

    [StringLength(160)]
    public string? Position { get; set; }

    /// <summary>Must be a valid ContactStatus value. Defaults to Potential.</summary>
    [EnumDataType(typeof(ContactStatus), ErrorMessage = "Unknown status.")]
    public ContactStatus Status { get; set; } = ContactStatus.Potential;

    [StringLength(300)]
    public string? NextAction { get; set; }
    public DateTimeOffset? NextActionDate { get; set; }

    public Guid? CompanyId { get; set; }
    public Guid? AssignedToId { get; set; }
}

public class UpdateContactDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(120, MinimumLength = 1)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>May be empty for company-style contacts entered as one word.</summary>
    [StringLength(120)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(256)]
    public string? Email { get; set; }

    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "Phone must be 7-15 digits, optionally starting with '+'.")]
    public string? Phone { get; set; }

    [StringLength(160)]
    public string? Position { get; set; }

    [EnumDataType(typeof(ContactStatus), ErrorMessage = "Unknown status.")]
    public ContactStatus Status { get; set; }

    [StringLength(300)]
    public string? NextAction { get; set; }
    public DateTimeOffset? NextActionDate { get; set; }

    public Guid? CompanyId { get; set; }
    public Guid? AssignedToId { get; set; }
}

/// <summary>Payload for the dedicated status-change endpoint. Just a field update.</summary>
public class UpdateContactStatusDto
{
    [EnumDataType(typeof(ContactStatus), ErrorMessage = "Unknown status.")]
    public ContactStatus Status { get; set; }
}

/// <summary>Payload for setting / clearing a contact's next action.</summary>
public class UpdateNextActionDto
{
    [StringLength(300, ErrorMessage = "Next action must be 300 characters or fewer.")]
    public string? NextAction { get; set; }

    public DateTimeOffset? NextActionDate { get; set; }
}
