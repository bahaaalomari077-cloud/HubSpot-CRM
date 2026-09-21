using Crm.Application.Common.Models;
using Crm.Application.Contacts;
using Crm.Application.Contacts.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _service;

    public ContactsController(IContactService service) => _service = service;

    /// <summary>List contacts with paging, search and filtering.</summary>
    /// <remarks>GET /api/contacts?page=1&amp;pageSize=20&amp;search=ali&amp;companyId=...&amp;sortBy=lastName&amp;sortDescending=true</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContactDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll(
        [FromQuery] ContactQueryParameters query,
        [FromQuery] string? groupBy,
        CancellationToken ct)
    {
        // ?groupBy=status returns { "potential": [...], "contacted": [...], ... }
        // for the kanban board, instead of a paged list.
        if (string.Equals(groupBy, "status", StringComparison.OrdinalIgnoreCase))
            return Ok(await _service.GetGroupedByStatusAsync(ct));

        return Ok(await _service.GetPagedAsync(query, ct));
    }

    /// <summary>Get a single contact with its company and owner.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> GetById(Guid id, CancellationToken ct)
    {
        var contact = await _service.GetByIdAsync(id, ct);
        return contact is null ? NotFound() : Ok(contact);
    }

    /// <summary>
    /// Import a pasted batch of contacts. Rows that fail validation are skipped
    /// with a reason; the rest still import.
    /// </summary>
    /// <remarks>POST /api/contacts/bulk-import</remarks>
    [HttpPost("bulk-import")]
    [ProducesResponseType(typeof(BulkImportResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkImportResultDto>> BulkImport(
        [FromBody] BulkImportDto dto, CancellationToken ct)
        => Ok(await _service.BulkImportAsync(dto, ct));

    /// <summary>Create a contact.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ContactDto>> Create(
        [FromBody] CreateContactDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update an existing contact.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> Update(
        Guid id, [FromBody] UpdateContactDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Move a contact to another status. Manual — any status can move to any other,
    /// no workflow rules.
    /// </summary>
    /// <remarks>PATCH /api/contacts/{id}/status  { "status": "Interested" }</remarks>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> UpdateStatus(
        Guid id, [FromBody] UpdateContactStatusDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateStatusAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Set or clear the contact's next action. Send an empty NextAction to clear it.
    /// </summary>
    /// <remarks>PATCH /api/contacts/{id}/next-action  { "nextAction": "Send quote", "nextActionDate": "2026-09-25T10:00:00Z" }</remarks>
    [HttpPatch("{id:guid}/next-action")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> UpdateNextAction(
        Guid id, [FromBody] UpdateNextActionDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateNextActionAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Soft-delete a contact.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
