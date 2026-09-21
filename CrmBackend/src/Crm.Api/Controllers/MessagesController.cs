using Crm.Application.Messaging;
using Crm.Application.Messaging.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Crm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MessagesController : ControllerBase
{
    private readonly IMessagingService _messaging;

    public MessagesController(IMessagingService messaging) => _messaging = messaging;

    /// <summary>
    /// Send an outbound message via Email (SMTP) or WhatsApp. The message is stored
    /// first, then dispatched; the response carries the resulting delivery status.
    /// </summary>
    /// <remarks>POST /api/messages/send</remarks>
    [HttpPost("send")]
    [ProducesResponseType(typeof(SendMessageResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SendMessageResultDto>> Send(
        [FromBody] SendMessageDto dto, CancellationToken ct)
    {
        var result = await _messaging.SendAsync(dto, ct);
        // 200 even on a provider failure — the attempt was recorded; status says what happened.
        return Ok(result);
    }
}
