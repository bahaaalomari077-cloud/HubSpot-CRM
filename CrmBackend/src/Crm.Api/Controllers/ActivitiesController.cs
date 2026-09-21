using Crm.Application.Activities;
using Crm.Application.Activities.Dtos;
using Crm.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ActivitiesController : ControllerBase
{
    private readonly IActivityService _service;

    public ActivitiesController(IActivityService service) => _service = service;

    /// <summary>List activities with paging and filtering (type, contactId, applicationId).</summary>
    /// <remarks>GET /api/activities?contactId=…&amp;type=7&amp;page=1&amp;pageSize=20</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ActivityDto>>> GetAll(
        [FromQuery] ActivityQueryParameters query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    /// <summary>Get a single activity with its contact and application.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityDto>> GetById(Guid id, CancellationToken ct)
    {
        var activity = await _service.GetByIdAsync(id, ct);
        return activity is null ? NotFound() : Ok(activity);
    }

    /// <summary>Create an activity (note / call / meeting …). Appears in the contact's timeline.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ActivityDto>> Create(
        [FromBody] CreateActivityDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an activity.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityDto>> Update(
        Guid id, [FromBody] UpdateActivityDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Soft-delete an activity (also removes it from the timeline).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
