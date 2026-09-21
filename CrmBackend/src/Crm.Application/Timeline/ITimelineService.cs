using Crm.Application.Timeline.Dtos;

namespace Crm.Application.Timeline;

public interface ITimelineService
{
    /// <summary>
    /// Merged, date-descending, date-grouped timeline of a contact's activities
    /// and messages.
    /// </summary>
    Task<IReadOnlyList<TimelineGroupDto>> GetForContactAsync(Guid contactId, CancellationToken ct = default);
}
