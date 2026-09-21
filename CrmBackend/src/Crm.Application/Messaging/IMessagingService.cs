using Crm.Application.Messaging.Dtos;

namespace Crm.Application.Messaging;

public interface IMessagingService
{
    /// <summary>
    /// Persist an outbound message, dispatch it through the channel's provider,
    /// and record the resulting status. Always returns a result — never throws on
    /// an expected provider failure (the failure is stored on the message).
    /// </summary>
    Task<SendMessageResultDto> SendAsync(SendMessageDto dto, CancellationToken ct = default);
}
