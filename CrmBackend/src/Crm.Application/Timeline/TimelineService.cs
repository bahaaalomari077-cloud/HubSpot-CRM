using Crm.Application.Common.Interfaces;
using Crm.Application.Timeline.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Timeline;

public class TimelineService : ITimelineService
{
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
                Tag = a.Type.ToString(),
                IsCompleted = a.IsCompleted
            })
            .ToListAsync(ct);

        var messages = await _db.Messages.AsNoTracking()
            .Where(m => m.ContactId == contactId)
            .Select(m => new TimelineItemDto
            {
                Id = m.Id,
                Kind = TimelineItemKind.Message,
                OccurredAt = m.SentAt,
                Title = m.Subject ?? m.Channel.ToString(),
                Detail = m.Body,
                Tag = m.Channel.ToString(),
                Direction = m.Direction.ToString()
            })
            .ToListAsync(ct);

        // --- merge + sort descending by moment ---
        var merged = activities
            .Concat(messages)
            .OrderByDescending(i => i.OccurredAt)
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
