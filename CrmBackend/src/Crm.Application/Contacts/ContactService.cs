using Crm.Application.Common.Interfaces;
using Crm.Application.Common.Models;
using Crm.Application.Contacts.Dtos;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Contacts;

public class ContactService : IContactService
{
    private readonly ICrmDbContext _db;
    private readonly ILogger<ContactService> _log;

    public ContactService(ICrmDbContext db, ILogger<ContactService> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<PagedResult<ContactDto>> GetPagedAsync(ContactQueryParameters query, CancellationToken ct = default)
    {
        // Start from a no-tracking query — read path never needs change tracking.
        var q = _db.Contacts.AsNoTracking().AsQueryable();

        // ---- Filtering ----
        if (query.CompanyId is { } companyId)
            q = q.Where(c => c.CompanyId == companyId);

        if (query.AssignedToId is { } assignedTo)
            q = q.Where(c => c.AssignedToId == assignedTo);

        if (query.Status is { } status)
            q = q.Where(c => (int)c.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            // Provider-agnostic case-insensitive match (EF translates ToLower/Contains to SQL).
            var term = query.Search.Trim().ToLower();
            q = q.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                (c.FirstName + " " + c.LastName).ToLower().Contains(term) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(term)) ||
                (c.Company != null && c.Company.Name.ToLower().Contains(term)));
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

        // The related names are projected in the same query — the list shows a
        // contact's company, and a second round-trip per row would be wasteful.
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                // Trimmed so a blank last name (company-only cards) leaves no trailing space.
                FullName = (c.FirstName + " " + c.LastName).Trim(),
                Email = c.Email,
                Phone = c.Phone,
                Position = c.Position,
                Status = c.Status,
                NextAction = c.NextAction,
                NextActionDate = c.NextActionDate,
                CompanyId = c.CompanyId,
                CompanyName = c.Company != null ? c.Company.Name : null,
                AssignedToId = c.AssignedToId,
                AssignedToName = c.AssignedTo != null ? c.AssignedTo.FullName : null,
                CreatedAt = c.CreatedAt
            })
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
            Status = dto.Status,
            NextAction = dto.NextAction,
            NextActionDate = dto.NextActionDate,
            CompanyId = dto.CompanyId,
            AssignedToId = dto.AssignedToId
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync(ct);

        _log.LogInformation("Contact created {ContactId} '{FullName}' status={Status}",
            contact.Id, contact.FullName, contact.Status);

        // Re-read with relations so the response carries CompanyName — the client
        // shows the new contact straight from this payload.
        var saved = await _db.Contacts
            .Include(c => c.Company)
            .Include(c => c.AssignedTo)
            .AsNoTracking()
            .FirstAsync(c => c.Id == contact.Id, ct);

        return MapWithRelations(saved);
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
        contact.Status = dto.Status;
        contact.NextAction = dto.NextAction;
        contact.NextActionDate = dto.NextActionDate;
        contact.CompanyId = dto.CompanyId;
        contact.AssignedToId = dto.AssignedToId;
        contact.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        // The company may have just changed, so the contact is re-read with its
        // relations — otherwise the response carries a stale name, or none at all.
        var saved = await _db.Contacts
            .Include(c => c.Company)
            .Include(c => c.AssignedTo)
            .AsNoTracking()
            .FirstAsync(c => c.Id == id, ct);

        return MapWithRelations(saved);
    }

    /// <summary>Manual status change. No rules — any status can move to any other.</summary>
    public async Task<ContactDto?> UpdateStatusAsync(Guid id, UpdateContactStatusDto dto, CancellationToken ct = default)
    {
        // Relations are included so the returned DTO keeps CompanyName/AssignedToName —
        // the client replaces its whole contact object with this response.
        var contact = await _db.Contacts
            .Include(c => c.Company)
            .Include(c => c.AssignedTo)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (contact is null) return null;

        var previous = contact.Status;
        contact.Status = dto.Status;
        contact.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        _log.LogInformation("Contact {ContactId} status {From} -> {To}",
            contact.Id, previous, contact.Status);

        return MapWithRelations(contact);
    }

    /// <summary>
    /// Import a pasted batch. Invalid rows are skipped with a reason rather than
    /// failing the whole import — a bad line shouldn't lose the other 49.
    /// </summary>
    public async Task<BulkImportResultDto> BulkImportAsync(BulkImportDto dto, CancellationToken ct = default)
    {
        var result = new BulkImportResultDto();
        var toAdd = new List<Contact>();
        var emailValidator = new System.ComponentModel.DataAnnotations.EmailAddressAttribute();

        // Existing emails, so a re-paste doesn't duplicate people.
        var incomingEmails = dto.Rows
            .Where(r => !string.IsNullOrWhiteSpace(r.Email))
            .Select(r => r.Email!.Trim().ToLower())
            .ToHashSet();

        var existingEmails = incomingEmails.Count == 0
            ? new HashSet<string>()
            : (await _db.Contacts.AsNoTracking()
                .Where(c => c.Email != null && incomingEmails.Contains(c.Email.ToLower()))
                .Select(c => c.Email!)
                .ToListAsync(ct))
              .Select(e => e.ToLower())
              .ToHashSet();

        var seenInBatch = new HashSet<string>();
        var line = 0;

        foreach (var row in dto.Rows)
        {
            line++;
            var name = (row.Name ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                result.Skipped++;
                result.Errors.Add($"Line {line}: missing name.");
                continue;
            }

            var email = string.IsNullOrWhiteSpace(row.Email) ? null : row.Email.Trim();
            if (email is not null && !emailValidator.IsValid(email))
            {
                result.Skipped++;
                result.Errors.Add($"Line {line} ({name}): '{email}' is not a valid email.");
                continue;
            }

            if (email is not null)
            {
                var key = email.ToLower();
                if (existingEmails.Contains(key))
                {
                    result.Skipped++;
                    result.Errors.Add($"Line {line} ({name}): a contact with {email} already exists.");
                    continue;
                }
                if (!seenInBatch.Add(key))
                {
                    result.Skipped++;
                    result.Errors.Add($"Line {line} ({name}): {email} appears twice in this paste.");
                    continue;
                }
            }

            // "Omar Tech" -> first "Omar", last "Tech"; a single word gets an em dash.
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            toAdd.Add(new Contact
            {
                FirstName = parts[0],
                LastName = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : string.Empty,
                Email = email,
                Phone = string.IsNullOrWhiteSpace(row.Phone) ? null : row.Phone.Trim(),
                Status = dto.Status
            });
        }

        if (toAdd.Count > 0)
        {
            _db.Contacts.AddRange(toAdd);
            await _db.SaveChangesAsync(ct);          // one round trip for the batch
            result.Created = toAdd.Count;
            result.Contacts = toAdd.Select(Map).ToList();
            _log.LogInformation("Bulk import created {Count} contacts, skipped {Skipped}",
                result.Created, result.Skipped);
        }

        return result;
    }

    /// <summary>
    /// All contacts grouped by status, for the kanban board. One query, grouped
    /// in memory — the board shows every contact, so there is nothing to page.
    /// </summary>
    public async Task<Dictionary<string, List<ContactDto>>> GetGroupedByStatusAsync(CancellationToken ct = default)
    {
        var contacts = await _db.Contacts.AsNoTracking()
            .Include(c => c.Company)
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .ToListAsync(ct);

        // Start with every status present, so empty columns still come back as [].
        var grouped = Enum.GetValues<ContactStatus>()
            .ToDictionary(
                st => char.ToLowerInvariant(st.ToString()[0]) + st.ToString()[1..],
                _ => new List<ContactDto>());

        foreach (var c in contacts)
        {
            var key = char.ToLowerInvariant(c.Status.ToString()[0]) + c.Status.ToString()[1..];
            grouped[key].Add(MapWithRelations(c));
        }

        return grouped;
    }

    /// <summary>Set or clear what to do next with this contact.</summary>
    public async Task<ContactDto?> UpdateNextActionAsync(Guid id, UpdateNextActionDto dto, CancellationToken ct = default)
    {
        var contact = await _db.Contacts
            .Include(c => c.Company)
            .Include(c => c.AssignedTo)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (contact is null) return null;

        contact.NextAction = string.IsNullOrWhiteSpace(dto.NextAction) ? null : dto.NextAction.Trim();
        contact.NextActionDate = contact.NextAction is null ? null : dto.NextActionDate;
        contact.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);

        _log.LogInformation("Contact {ContactId} next action set to '{NextAction}' due {Due}",
            contact.Id, contact.NextAction ?? "(none)", contact.NextActionDate);

        return MapWithRelations(contact);
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
        Status = c.Status,
        NextAction = c.NextAction,
        NextActionDate = c.NextActionDate,
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
