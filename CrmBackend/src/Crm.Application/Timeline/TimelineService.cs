using Crm.Application.Common.Interfaces;
using Crm.Application.Timeline.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Timeline;

public class TimelineService : ITimelineService
{
    /// <summary>
    /// Cap rows pulled from each source, so a contact with thousands of messages
    /// doesn't load them all into memory. The merged list is sorted afterwards.
    /// </summary>
    private const int MaxPerSource = 50;

    /// <summary>Total entries returned across both sources.</summary>
    private const int MaxTotal = 50;

    /// <summary>Message bodies are truncated to a preview in the timeline.</summary>
    private const int BodyPreviewLength = 280;

    private readonly ICrmDbContext _db;

    public TimelineService(ICrmDbContext db) => _db = db;

    public async Task<IReadOnlyList<TimelineGroupDto>> GetForContactAsync(
        Guid contactId, CancellationToken ct = default)
    {
        // --- pull both streams, each projected to the common timeline shape ---
        // Projected in-DB to only the columns we need, then materialized.
        var activities = await _db.Activities.AsNoTracking()
            .Where(a => a.ContactId == contactId)
            .Select(a => new TimelineItemDto
            {
                Id = a.Id,
                Kind = TimelineItemKind.Activity,
                OccurredAt = a.CreatedAt,
                Title = a.Subject,
                Detail = a.Description,
                SubType = a.Type.ToString(),   // Call | Meeting | Note | Task …
                IsCompleted = a.IsCompleted
            })
            .OrderByDescending(i => i.OccurredAt)
            .Take(MaxPerSource)
            .ToListAsync(ct);

        var messages = await _db.Messages.AsNoTracking()
            .Where(m => m.ContactId == contactId)
            .Select(m => new TimelineItemDto
            {
                Id = m.Id,
                Kind = TimelineItemKind.Message,
                OccurredAt = m.SentAt,
                Title = m.Subject ?? m.Channel.ToString(),
                // Preview only — don't ship full email bodies to the timeline.
                Detail = m.Body.Length > BodyPreviewLength
                    ? m.Body.Substring(0, BodyPreviewLength)
                    : m.Body,
                SubType = m.Channel.ToString(),  // Email | WhatsApp | Sms | Telegram …
                Direction = m.Direction.ToString()
            })
            .OrderByDescending(i => i.OccurredAt)
            .Take(MaxPerSource)
            .ToListAsync(ct);

        // --- merge + sort descending by moment ---
        var merged = activities
            .Concat(messages)
            .OrderByDescending(i => i.OccurredAt)
            .Take(MaxTotal)          // 50 most recent entries overall
            .ToList();

        // --- group by calendar date (in the item's own offset) ---
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime);

        var groups = merged
            .GroupBy(i => DateOnly.FromDateTime(i.OccurredAt.UtcDateTime))
            .OrderByDescending(g => g.Key)
            .Select(g => new TimelineGroupDto
            {
                Date = g.Key,
                Label = LabelFor(g.Key, today),
                Items = g.ToList()   // already in descending order from the sort above
            })
            .ToList();

        return groups;
    }

    private static string LabelFor(DateOnly date, DateOnly today)
    {
        if (date == today) return "Today";
        if (date == today.AddDays(-1)) return "Yesterday";
        return date.ToString("MMMM d, yyyy");
    }
}
