using Crm.Application.Common.Models;
using Crm.Application.Companies;
using Crm.Application.Companies.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _service;

    public CompaniesController(ICompanyService service) => _service = service;

    /// <summary>List companies with paging and search.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<CompanyDto>>> GetAll(
        [FromQuery] CompanyQueryParameters query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    /// <summary>Get a single company, including how many contacts it has.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> GetById(Guid id, CancellationToken ct)
    {
        var company = await _service.GetByIdAsync(id, ct);
        return company is null ? NotFound() : Ok(company);
    }

    /// <summary>
    /// Company details: the company, its contacts, and a status breakdown —
    /// everything the details page needs in one call.
    /// </summary>
    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(typeof(CompanyDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDetailsDto>> GetDetails(Guid id, CancellationToken ct)
    {
        var details = await _service.GetDetailsAsync(id, ct);
        return details is null ? NotFound() : Ok(details);
    }

    /// <summary>Create a company.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CompanyDto>> Create(
        [FromBody] CreateCompanyDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Update a company.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> Update(
        Guid id, [FromBody] UpdateCompanyDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Soft-delete a company. Its contacts are kept, just unlinked.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
