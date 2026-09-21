using Crm.Domain.Common;

namespace Crm.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    // Address (kept flat like Bitrix24 to allow geo-filtering).
    public string? AddressCity { get; set; }
    public string? AddressCountry { get; set; }

    public decimal? AnnualRevenue { get; set; }
    public int? EmployeeCount { get; set; }

    // Responsible person.
    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    // One company has many contacts and many applications.
    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
