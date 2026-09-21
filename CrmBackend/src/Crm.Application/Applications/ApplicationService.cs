using Crm.Application.Applications.Dtos;
using Crm.Application.Common.Interfaces;
using Crm.Application.Common.Models;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Crm.Domain.Entities.Application;

namespace Crm.Application.Applications;

public class ApplicationService : IApplicationService
{
    private readonly ICrmDbContext _db;

    public ApplicationService(ICrmDbContext db) => _db = db;

    public async Task<PagedResult<ApplicationDto>> GetPagedAsync(ApplicationQueryParameters query, CancellationToken ct = default)
    {
        var q = _db.Applications.AsNoTracking().AsQueryable();

        // ---- Filtering ----
        if (query.Stage is { } stage)
            q = q.Where(a => (int)a.Stage == stage);

        if (query.CompanyId is { } companyId)
            q = q.Where(a => a.CompanyId == companyId);

        if (query.ContactId is { } contactId)
            q = q.Where(a => a.ContactId == contactId);

        if (query.AssignedToId is { } assignedTo)
            q = q.Where(a => a.AssignedToId == assignedTo);

        if (query.IsClosed is { } isClosed)
            q = q.Where(a => a.IsClosed == isClosed);

        if (query.MinAmount is { } min)
            q = q.Where(a => a.Amount >= min);

        if (query.MaxAmount is { } max)
            q = q.Where(a => a.Amount <= max);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a => a.Title.ToLower().Contains(term));
        }

        // ---- Sorting ----
        q = (query.SortBy?.ToLowerInvariant(), query.SortDescending) switch
        {
            ("title", false)  => q.OrderBy(a => a.Title),
            ("title", true)   => q.OrderByDescending(a => a.Title),
            ("amount", false) => q.OrderBy(a => a.Amount),
            ("amount", true)  => q.OrderByDescending(a => a.Amount),
            ("stage", false)  => q.OrderBy(a => a.Stage),
            ("stage", true)   => q.OrderByDescending(a => a.Stage),
            (_, true)         => q.OrderByDescending(a => a.CreatedAt),
            _                 => q.OrderBy(a => a.CreatedAt)
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => Map(a))
            .ToListAsync(ct);

        return new PagedResult<ApplicationDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<ApplicationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var app = await _db.Applications.AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.Company)
            .Include(a => a.AssignedTo)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return app is null ? null : MapWithRelations(app);
    }

    public async Task<ApplicationDto> CreateAsync(CreateApplicationDto dto, CancellationToken ct = default)
    {
        var app = new ApplicationEntity
        {
            Title = dto.Title,
            Amount = dto.Amount,
            CurrencyCode = dto.CurrencyCode,
            Probability = dto.Probability,
            Stage = dto.Stage,
            StageSemantic = SemanticFor(dto.Stage),
            IsClosed = IsTerminal(dto.Stage),
            ContactId = dto.ContactId,
            CompanyId = dto.CompanyId,
            AssignedToId = dto.AssignedToId,
            BeginDate = DateTimeOffset.UtcNow,
            CloseDate = DateTimeOffset.UtcNow.AddDays(7) // Bitrix24 default: +7 days
        };

        _db.Applications.Add(app);
        await _db.SaveChangesAsync(ct);
        return Map(app);
    }

    public async Task<ApplicationDto?> UpdateAsync(Guid id, UpdateApplicationDto dto, CancellationToken ct = default)
    {
        var app = await _db.Applications.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (app is null) return null;

        app.Title = dto.Title;
        app.Amount = dto.Amount;
        app.CurrencyCode = dto.CurrencyCode;
        app.Probability = dto.Probability;
        app.ContactId = dto.ContactId;
        app.CompanyId = dto.CompanyId;
        app.AssignedToId = dto.AssignedToId;
        app.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(app);
    }

    /// <summary>
    /// Kanban stage move. Reproduces the live-verified Bitrix24 behavior:
    /// semantic recomputes, IsClosed flips, and CloseDate resets to now on a win/loss.
    /// </summary>
    public async Task<ApplicationDto?> MoveStageAsync(Guid id, MoveStageDto dto, CancellationToken ct = default)
    {
        var app = await _db.Applications.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (app is null) return null;

        app.Stage = dto.Stage;
        app.StageSemantic = SemanticFor(dto.Stage);
        app.IsClosed = IsTerminal(dto.Stage);
        if (app.IsClosed)
            app.CloseDate = DateTimeOffset.UtcNow;
        app.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(app);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var app = await _db.Applications.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (app is null) return false;

        app.IsDeleted = true;
        app.DeletedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static StageSemantic SemanticFor(ApplicationStage stage) => stage switch
    {
        ApplicationStage.Won => StageSemantic.Success,
        ApplicationStage.Lost => StageSemantic.Failure,
        _ => StageSemantic.Process
    };

    private static bool IsTerminal(ApplicationStage stage) =>
        stage is ApplicationStage.Won or ApplicationStage.Lost;

    private static ApplicationDto Map(ApplicationEntity a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Stage = a.Stage,
        StageSemantic = a.StageSemantic,
        IsClosed = a.IsClosed,
        Amount = a.Amount,
        CurrencyCode = a.CurrencyCode,
        Probability = a.Probability,
        BeginDate = a.BeginDate,
        CloseDate = a.CloseDate,
        ContactId = a.ContactId,
        CompanyId = a.CompanyId,
        AssignedToId = a.AssignedToId,
        CreatedAt = a.CreatedAt
    };

    private static ApplicationDto MapWithRelations(ApplicationEntity a)
    {
        var dto = Map(a);
        dto.ContactName = a.Contact?.FullName;
        dto.CompanyName = a.Company?.Name;
        dto.AssignedToName = a.AssignedTo?.FullName;
        return dto;
    }
}
