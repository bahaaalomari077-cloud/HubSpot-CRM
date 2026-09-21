namespace Crm.Application.Timeline.Dtos;

/// <summary>The kind of record a timeline entry came from.</summary>
public enum TimelineItemKind
{
    Activity = 0,
    Message = 1
}

/// <summary>
/// One normalized entry in a contact's timeline — an activity or a message,
/// flattened to a common shape so the two streams can be merged and sorted together.
/// </summary>
public class TimelineItemDto
{
    public Guid Id { get; set; }
    public TimelineItemKind Kind { get; set; }

    /// <summary>The moment the entry belongs at on the timeline (activity created / message sent).</summary>
    public DateTimeOffset OccurredAt { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }

    /// <summary>
    /// Sub-type used to pick an icon.
    /// Activities → "Call", "Meeting", "Note", "Task" (ActivityType).
    /// Messages   → "Email", "WhatsApp", "Sms", "Telegram" (MessageChannel).
    /// </summary>
    public string SubType { get; set; } = string.Empty;

    /// <summary>For messages: "Inbound"/"Outbound"; null for activities.</summary>
    public string? Direction { get; set; }

    /// <summary>For activities: completion state; null for messages.</summary>
    public bool? IsCompleted { get; set; }
}

/// <summary>A day-bucket of timeline items, for the "group by date" requirement.</summary>
public class TimelineGroupDto
{
    public DateOnly Date { get; set; }

    /// <summary>Human label the UI can show directly: "Today", "Yesterday", or the date.</summary>
    public string Label { get; set; } = string.Empty;

    public List<TimelineItemDto> Items { get; set; } = new();
}
