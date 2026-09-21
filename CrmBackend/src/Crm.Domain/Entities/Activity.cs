using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

/// <summary>
/// A scheduled thing someone must do (call, meeting, task). Distinct from
/// Message, which is a record of a communication that already happened.
/// </summary>
public class Activity : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ActivityType Type { get; set; } = ActivityType.Task;
    public ActivityStatus Status { get; set; } = ActivityStatus.Pending;

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? Deadline { get; set; }
    public bool IsCompleted { get; set; }

    // Owner links — an activity hangs off a contact and optionally an application.
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public Guid? ApplicationId { get; set; }
    public Application? Application { get; set; }

    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
}
