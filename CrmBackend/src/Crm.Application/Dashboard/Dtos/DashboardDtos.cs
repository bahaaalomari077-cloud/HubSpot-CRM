using Crm.Domain.Enums;

namespace Crm.Application.Dashboard.Dtos;

/// <summary>How many contacts sit in each status.</summary>
public class StatusGroupDto
{
    public ContactStatus Status { get; set; }
    public int Count { get; set; }
}

/// <summary>A contact with a planned next action, for the "what's next" list.</summary>
public class UpcomingActionDto
{
    public Guid ContactId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public ContactStatus Status { get; set; }
    public string NextAction { get; set; } = string.Empty;
    public DateTimeOffset? NextActionDate { get; set; }

    /// <summary>True when the due date has already passed.</summary>
    public bool IsOverdue { get; set; }

    /// <summary>Negative = days overdue, 0 = today, positive = days remaining.</summary>
    public int? DaysUntil { get; set; }
}

/// <summary>A recent activity or message, across all contacts.</summary>
public class RecentItemDto
{
    public Guid Id { get; set; }
    public string Kind { get; set; } = string.Empty;      // Activity | Message
    public string SubType { get; set; } = string.Empty;   // Call | Email | WhatsApp | Note
    public string Title { get; set; } = string.Empty;
    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}

/// <summary>Everything the dashboard needs, in one call.</summary>
public class DashboardDto
{
    public List<StatusGroupDto> ContactsByStatus { get; set; } = new();
    public List<UpcomingActionDto> UpcomingActions { get; set; } = new();
    public List<RecentItemDto> RecentItems { get; set; } = new();

    public int TotalContacts { get; set; }
    public int OverdueCount { get; set; }
}
