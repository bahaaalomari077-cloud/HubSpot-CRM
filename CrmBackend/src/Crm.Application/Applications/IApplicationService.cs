using Crm.Application.Applications.Dtos;
using Crm.Application.Common.Models;

namespace Crm.Application.Applications;

public interface IApplicationService
{
    Task<PagedResult<ApplicationDto>> GetPagedAsync(ApplicationQueryParameters query, CancellationToken ct = default);
    Task<ApplicationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ApplicationDto> CreateAsync(CreateApplicationDto dto, CancellationToken ct = default);
    Task<ApplicationDto?> UpdateAsync(Guid id, UpdateApplicationDto dto, CancellationToken ct = default);
    Task<ApplicationDto?> MoveStageAsync(Guid id, MoveStageDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
