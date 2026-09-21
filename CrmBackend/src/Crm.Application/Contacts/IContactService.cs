using Crm.Application.Common.Models;
using Crm.Application.Contacts.Dtos;

namespace Crm.Application.Contacts;

public interface IContactService
{
    Task<PagedResult<ContactDto>> GetPagedAsync(ContactQueryParameters query, CancellationToken ct = default);
    Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ContactDto> CreateAsync(CreateContactDto dto, CancellationToken ct = default);
    Task<ContactDto?> UpdateAsync(Guid id, UpdateContactDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
