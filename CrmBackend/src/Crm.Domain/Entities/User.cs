using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Agent;
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Records this user is the responsible owner of.
    public ICollection<Contact> AssignedContacts { get; set; } = new List<Contact>();
    public ICollection<Company> AssignedCompanies { get; set; } = new List<Company>();
    public ICollection<Application> AssignedApplications { get; set; } = new List<Application>();
    public ICollection<Activity> AssignedActivities { get; set; } = new List<Activity>();
}
