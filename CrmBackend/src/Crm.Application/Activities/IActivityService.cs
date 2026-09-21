using Crm.Application.Activities.Dtos;
using Crm.Application.Common.Models;

namespace Crm.Application.Activities;

public interface IActivityService
{
    Task<PagedResult<ActivityDto>> GetPagedAsync(ActivityQueryParameters query, CancellationToken ct = default);
    Task<ActivityDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ActivityDto> CreateAsync(CreateActivityDto dto, CancellationToken ct = default);
    Task<ActivityDto?> UpdateAsync(Guid id, UpdateActivityDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
