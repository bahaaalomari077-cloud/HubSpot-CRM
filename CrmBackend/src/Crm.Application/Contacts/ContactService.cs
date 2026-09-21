using Crm.Application.Common.Interfaces;
using Crm.Application.Common.Models;
using Crm.Application.Contacts.Dtos;
using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.Contacts;

public class ContactService : IContactService
{
    private readonly ICrmDbContext _db;

    public ContactService(ICrmDbContext db) => _db = db;

    public async Task<PagedResult<ContactDto>> GetPagedAsync(ContactQueryParameters query, CancellationToken ct = default)
    {
        // Start from a no-tracking query — read path never needs change tracking.
        var q = _db.Contacts.AsNoTracking().AsQueryable();

        // ---- Filtering ----
        if (query.CompanyId is { } companyId)
            q = q.Where(c => c.CompanyId == companyId);

        if (query.AssignedToId is { } assignedTo)
            q = q.Where(c => c.AssignedToId == assignedTo);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            // Provider-agnostic case-insensitive match (EF translates ToLower/Contains to SQL).
            var term = query.Search.Trim().ToLower();
            q = q.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(term)));
        }

        // ---- Sorting (allow-list; unknown fields fall back to CreatedAt) ----
        q = (query.SortBy?.ToLowerInvariant(), query.SortDescending) switch
        {
            ("firstname", false) => q.OrderBy(c => c.FirstName),
            ("firstname", true)  => q.OrderByDescending(c => c.FirstName),
            ("lastname", false)  => q.OrderBy(c => c.LastName),
            ("lastname", true)   => q.OrderByDescending(c => c.LastName),
            ("email", false)     => q.OrderBy(c => c.Email),
            ("email", true)      => q.OrderByDescending(c => c.Email),
            (_, true)            => q.OrderByDescending(c => c.CreatedAt),
            _                    => q.OrderBy(c => c.CreatedAt)
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => Map(c))
            .ToListAsync(ct);

        return new PagedResult<ContactDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _db.Contacts.AsNoTracking()
            .Include(c => c.Company)
            .Include(c => c.AssignedTo)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return contact is null ? null : MapWithRelations(contact);
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto, CancellationToken ct = default)
    {
        var contact = new Contact
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Position = dto.Position,
            CompanyId = dto.CompanyId,
            AssignedToId = dto.AssignedToId
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync(ct);
        return Map(contact);
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto, CancellationToken ct = default)
    {
        var contact = await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (contact is null) return null;

        contact.FirstName = dto.FirstName;
        contact.LastName = dto.LastName;
        contact.Email = dto.Email;
        contact.Phone = dto.Phone;
        contact.Position = dto.Position;
        contact.CompanyId = dto.CompanyId;
        contact.AssignedToId = dto.AssignedToId;
        contact.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(contact);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var contact = await _db.Contacts.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (contact is null) return false;

        // Soft delete — the global query filter hides it from every read.
        contact.IsDeleted = true;
        contact.DeletedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ContactDto Map(Contact c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        FullName = c.FullName,
        Email = c.Email,
        Phone = c.Phone,
        Position = c.Position,
        CompanyId = c.CompanyId,
        AssignedToId = c.AssignedToId,
        CreatedAt = c.CreatedAt
    };

    private static ContactDto MapWithRelations(Contact c)
    {
        var dto = Map(c);
        dto.CompanyName = c.Company?.Name;
        dto.AssignedToName = c.AssignedTo?.FullName;
        return dto;
    }
}
