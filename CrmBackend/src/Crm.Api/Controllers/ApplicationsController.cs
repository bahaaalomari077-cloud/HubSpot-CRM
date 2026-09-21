using Crm.Application.Applications;
using Crm.Application.Applications.Dtos;
using Crm.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationsController(IApplicationService service) => _service = service;

    /// <summary>List applications (deals) with paging, search and filtering.</summary>
    /// <remarks>
    /// GET /api/applications?page=1&amp;pageSize=20&amp;stage=0&amp;isClosed=false
    ///     &amp;minAmount=1000&amp;maxAmount=50000&amp;sortBy=amount&amp;sortDescending=true
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ApplicationDto>>> GetAll(
        [FromQuery] ApplicationQueryParameters query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    /// <summary>Get a single application with its contact, company and owner.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id, CancellationToken ct)
    {
        var app = await _service.GetByIdAsync(id, ct);
        return app is null ? NotFound() : Ok(app);
    }

    /// <summary>Create an application.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApplicationDto>> Create(
        [FromBody] CreateApplicationDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an application's editable fields.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Update(
        Guid id, [FromBody] UpdateApplicationDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Move an application to a new pipeline stage (kanban drag).</summary>
    [HttpPatch("{id:guid}/stage")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> MoveStage(
        Guid id, [FromBody] MoveStageDto dto, CancellationToken ct)
    {
        var moved = await _service.MoveStageAsync(id, dto, ct);
        return moved is null ? NotFound() : Ok(moved);
    }

    /// <summary>Soft-delete an application.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
