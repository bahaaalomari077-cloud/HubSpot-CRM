using Crm.Application.Dashboard;
using Crm.Application.Dashboard.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>
    /// Everything the dashboard shows: contacts grouped by status, upcoming next
    /// actions (overdue first), and the 5 most recent timeline items.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken ct)
        => Ok(await _service.GetAsync(ct));
}
