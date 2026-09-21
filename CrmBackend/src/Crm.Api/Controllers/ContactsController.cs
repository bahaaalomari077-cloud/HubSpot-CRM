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
    public async Task<ActionResult<PagedResult<ContactDto>>> GetAll(
        [FromQuery] ContactQueryParameters query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    /// <summary>Get a single contact with its company and owner.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactDto>> GetById(Guid id, CancellationToken ct)
    {
        var contact = await _service.GetByIdAsync(id, ct);
        return contact is null ? NotFound() : Ok(contact);
    }

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

    /// <summary>Soft-delete a contact.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
