using Crm.Domain.Enums;

namespace Crm.Application.Activities.Dtos;

public class ActivityDto
{
    public Guid Id { get; set; }
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ActivityStatus Status { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? Deadline { get; set; }

    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public Guid? ApplicationId { get; set; }
    public string? ApplicationTitle { get; set; }
    public Guid? AssignedToId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateActivityDto
{
    /// <summary>note | call | meeting (or the other ActivityType values).</summary>
    public ActivityType Type { get; set; } = ActivityType.Note;

    /// <summary>Short title. If omitted, it's derived from the type.</summary>
    public string? Subject { get; set; }

    public string? Description { get; set; }

    public Guid ContactId { get; set; }
    public Guid? ApplicationId { get; set; }   // optional
    public Guid? AssignedToId { get; set; }

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? Deadline { get; set; }
}

public class UpdateActivityDto
{
    public ActivityType Type { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public ActivityStatus Status { get; set; }
    public bool IsCompleted { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? AssignedToId { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? Deadline { get; set; }
}
