using Crm.Application.Common.Models;
using Crm.Application.Contacts.Dtos;

namespace Crm.Application.Contacts;

public interface IContactService
{
    Task<PagedResult<ContactDto>> GetPagedAsync(ContactQueryParameters query, CancellationToken ct = default);
    Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ContactDto> CreateAsync(CreateContactDto dto, CancellationToken ct = default);
    Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto, CancellationToken ct = default);

    /// <summary>Manual status change — just a field update, no workflow rules.</summary>
    Task<ContactDto?> UpdateStatusAsync(Guid id, UpdateContactStatusDto dto, CancellationToken ct = default);

    /// <summary>Import a pasted batch of contacts; bad rows are skipped with reasons.</summary>
    Task<BulkImportResultDto> BulkImportAsync(BulkImportDto dto, CancellationToken ct = default);

    /// <summary>All contacts grouped by status, for the kanban board.</summary>
    Task<Dictionary<string, List<ContactDto>>> GetGroupedByStatusAsync(CancellationToken ct = default);

    /// <summary>Set or clear the contact's next action and its due date.</summary>
    Task<ContactDto?> UpdateNextActionAsync(Guid id, UpdateNextActionDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
