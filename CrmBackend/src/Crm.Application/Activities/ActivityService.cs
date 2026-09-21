using Crm.Application.Activities.Dtos;
using Crm.Application.Common.Interfaces;
using Crm.Application.Common.Models;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Activities;

public class ActivityService : IActivityService
{
    private readonly ICrmDbContext _db;
    private readonly ILogger<ActivityService> _log;

    public ActivityService(ICrmDbContext db, ILogger<ActivityService> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<PagedResult<ActivityDto>> GetPagedAsync(ActivityQueryParameters query, CancellationToken ct = default)
    {
        var q = _db.Activities.AsNoTracking().AsQueryable();

        // ---- Filtering ----
        if (query.Type is { } type)
            q = q.Where(a => (int)a.Type == type);

        if (query.ContactId is { } contactId)
            q = q.Where(a => a.ContactId == contactId);

        if (query.ApplicationId is { } appId)
            q = q.Where(a => a.ApplicationId == appId);

        if (query.AssignedToId is { } assignedTo)
            q = q.Where(a => a.AssignedToId == assignedTo);

        if (query.IsCompleted is { } completed)
            q = q.Where(a => a.IsCompleted == completed);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a =>
                a.Subject.ToLower().Contains(term) ||
                (a.Description != null && a.Description.ToLower().Contains(term)));
        }

        // ---- Sorting (default createdAt desc, matching the timeline) ----
        q = (query.SortBy?.ToLowerInvariant(), query.SortDescending) switch
        {
            ("subject", false)  => q.OrderBy(a => a.Subject),
            ("subject", true)   => q.OrderByDescending(a => a.Subject),
            ("deadline", false) => q.OrderBy(a => a.Deadline),
            ("deadline", true)  => q.OrderByDescending(a => a.Deadline),
            ("createdat", false)=> q.OrderBy(a => a.CreatedAt),
            _                   => q.OrderByDescending(a => a.CreatedAt)
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => Map(a))
            .ToListAsync(ct);

        return new PagedResult<ActivityDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var activity = await _db.Activities.AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.Application)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return activity is null ? null : MapWithRelations(activity);
    }

    public async Task<ActivityDto> CreateAsync(CreateActivityDto dto, CancellationToken ct = default)
    {
        var activity = new Activity
        {
            Type = dto.Type,
            Subject = string.IsNullOrWhiteSpace(dto.Subject) ? dto.Type.ToString() : dto.Subject!,
            Description = dto.Description,
            ContactId = dto.ContactId,
            ApplicationId = dto.ApplicationId,
            AssignedToId = dto.AssignedToId,
            StartTime = dto.StartTime,
            Deadline = dto.Deadline,
            Status = ActivityStatus.Pending
        };

        _db.Activities.Add(activity);
        await _db.SaveChangesAsync(ct);

        _log.LogInformation("Activity logged {ActivityId} type={Type} contact={ContactId}",
            activity.Id, activity.Type, activity.ContactId);

        return Map(activity);
    }

    public async Task<ActivityDto?> UpdateAsync(Guid id, UpdateActivityDto dto, CancellationToken ct = default)
    {
        var activity = await _db.Activities.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (activity is null) return null;

        activity.Type = dto.Type;
        if (!string.IsNullOrWhiteSpace(dto.Subject)) activity.Subject = dto.Subject!;
        activity.Description = dto.Description;
        activity.Status = dto.Status;
        activity.IsCompleted = dto.IsCompleted;
        activity.ApplicationId = dto.ApplicationId;
        activity.AssignedToId = dto.AssignedToId;
        activity.StartTime = dto.StartTime;
        activity.Deadline = dto.Deadline;
        activity.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(activity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var activity = await _db.Activities.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (activity is null) return false;

        // Soft delete — global query filter hides it, including from the timeline.
        activity.IsDeleted = true;
        activity.DeletedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ActivityDto Map(Activity a) => new()
    {
        Id = a.Id,
        Type = a.Type,
        Subject = a.Subject,
        Description = a.Description,
        Status = a.Status,
        IsCompleted = a.IsCompleted,
        StartTime = a.StartTime,
        Deadline = a.Deadline,
        ContactId = a.ContactId,
        ApplicationId = a.ApplicationId,
        AssignedToId = a.AssignedToId,
        CreatedAt = a.CreatedAt
    };

    private static ActivityDto MapWithRelations(Activity a)
    {
        var dto = Map(a);
        dto.ContactName = a.Contact?.FullName;
        dto.ApplicationTitle = a.Application?.Title;
        return dto;
    }
}
