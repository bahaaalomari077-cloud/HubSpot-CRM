using Crm.Application.Timeline;
using Crm.Application.Timeline.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class TimelineController : ControllerBase
{
    private readonly ITimelineService _service;

    public TimelineController(ITimelineService service) => _service = service;

    /// <summary>
    /// Merged timeline for a contact: activities + messages, sorted newest-first,
    /// grouped by date. One call, one shape.
    /// </summary>
    /// <remarks>GET /api/contacts/{contactId}/timeline</remarks>
    [HttpGet("contacts/{contactId:guid}/timeline")]
    [ProducesResponseType(typeof(IReadOnlyList<TimelineGroupDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TimelineGroupDto>>> GetForContact(
        Guid contactId, CancellationToken ct)
        => Ok(await _service.GetForContactAsync(contactId, ct));
}
