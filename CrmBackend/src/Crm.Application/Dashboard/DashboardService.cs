using Crm.Application.Common.Interfaces;
using Crm.Application.Dashboard.Dtos;
using Crm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(CancellationToken ct = default);
}

public class DashboardService : IDashboardService
{
    private const int UpcomingLimit = 8;
    private const int RecentLimit = 5;

    private readonly ICrmDbContext _db;

    public DashboardService(ICrmDbContext db) => _db = db;

    public async Task<DashboardDto> GetAsync(CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);

        // ---- 1. contacts grouped by status (single GROUP BY in SQL) ----
        var grouped = await _db.Contacts.AsNoTracking()
            .GroupBy(c => c.Status)
            .Select(g => new StatusGroupDto { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Fill in statuses with zero contacts so the UI always shows all six.
        var byStatus = Enum.GetValues<ContactStatus>()
            .Select(s => grouped.FirstOrDefault(g => g.Status == s)
                         ?? new StatusGroupDto { Status = s, Count = 0 })
            .ToList();

        // ---- 2. upcoming actions, soonest first (overdue included) ----
        var upcomingRaw = await _db.Contacts.AsNoTracking()
            .Where(c => c.NextAction != null && c.NextAction != "")
            .OrderBy(c => c.NextActionDate == null)        // dated ones first
            .ThenBy(c => c.NextActionDate)
            .Take(UpcomingLimit)
            .Select(c => new
            {
                c.Id, c.FirstName, c.LastName, c.Status, c.NextAction, c.NextActionDate
            })
            .ToListAsync(ct);

        var upcoming = upcomingRaw.Select(c =>
        {
            int? daysUntil = c.NextActionDate is { } due
                ? DateOnly.FromDateTime(due.UtcDateTime).DayNumber - today.DayNumber
                : null;

            return new UpcomingActionDto
            {
                ContactId = c.Id,
                ContactName = $"{c.FirstName} {c.LastName}".Trim(),
                Status = c.Status,
                NextAction = c.NextAction!,
                NextActionDate = c.NextActionDate,
                IsOverdue = daysUntil is < 0,
                DaysUntil = daysUntil
            };
        }).ToList();

        // ---- 3. recent timeline items across all contacts ----
        var recentActivities = await _db.Activities.AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Take(RecentLimit)
            .Select(a => new RecentItemDto
            {
                Id = a.Id,
                Kind = "Activity",
                SubType = a.Type.ToString(),
                Title = a.Subject,
                ContactId = a.ContactId,
                ContactName = a.Contact != null ? a.Contact.FirstName + " " + a.Contact.LastName : null,
                OccurredAt = a.CreatedAt
            })
            .ToListAsync(ct);

        var recentMessages = await _db.Messages.AsNoTracking()
            .OrderByDescending(m => m.SentAt)
            .Take(RecentLimit)
            .Select(m => new RecentItemDto
            {
                Id = m.Id,
                Kind = "Message",
                SubType = m.Channel.ToString(),
                Title = m.Subject ?? m.Channel.ToString(),
                ContactId = m.ContactId,
                ContactName = m.Contact != null ? m.Contact.FirstName + " " + m.Contact.LastName : null,
                OccurredAt = m.SentAt
            })
            .ToListAsync(ct);

        var recent = recentActivities
            .Concat(recentMessages)
            .OrderByDescending(i => i.OccurredAt)
            .Take(RecentLimit)
            .ToList();

        return new DashboardDto
        {
            ContactsByStatus = byStatus,
            UpcomingActions = upcoming,
            RecentItems = recent,
            TotalContacts = byStatus.Sum(s => s.Count),
            OverdueCount = upcoming.Count(u => u.IsOverdue)
        };
    }
}
