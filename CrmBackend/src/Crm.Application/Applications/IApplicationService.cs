using Crm.Application.Applications.Dtos;
using Crm.Application.Common.Models;

namespace Crm.Application.Applications;

public interface IApplicationService
{
    Task<PagedResult<ApplicationDto>> GetPagedAsync(ApplicationQueryParameters query, CancellationToken ct = default);
    Task<ApplicationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ApplicationDto> CreateAsync(CreateApplicationDto dto, CancellationToken ct = default);
    Task<ApplicationDto?> UpdateAsync(Guid id, UpdateApplicationDto dto, CancellationToken ct = default);
    /// <summary>
    /// Move an application to a new stage. Enforces the pipeline rules (no skipping,
    /// no backward unless opted in, terminal stages are final).
    /// </summary>
    Task<MoveStageResult> MoveStageAsync(Guid id, MoveStageDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
