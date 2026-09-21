using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

public class Contact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }

    /// <summary>Where this contact stands. Set manually by the admin.</summary>
    public ContactStatus Status { get; set; } = ContactStatus.Potential;

    /// <summary>What the admin plans to do next, e.g. "Send the revised quote".</summary>
    public string? NextAction { get; set; }

    /// <summary>When that next action is due. Drives the dashboard's upcoming list.</summary>
    public DateTimeOffset? NextActionDate { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // A contact may belong to a company (nullable — a contact can be independent).
    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    // Responsible person.
    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
