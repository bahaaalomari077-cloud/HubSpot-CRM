using System.Net.Http.Json;
using System.Text.Json;
using Crm.Application.Messaging.Providers;
using Crm.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Crm.Infrastructure.Messaging;

/// <summary>Sends WhatsApp messages via the Meta WhatsApp Cloud API.</summary>
public class WhatsAppProvider : IMessageProvider
{
    private readonly HttpClient _http;
    private readonly WhatsAppOptions _opt;
    private readonly ILogger<WhatsAppProvider> _log;

    public WhatsAppProvider(HttpClient http, IOptions<WhatsAppOptions> opt, ILogger<WhatsAppProvider> log)
    {
        _http = http;
        _opt = opt.Value;
        _log = log;
    }

    public MessageChannel Channel => MessageChannel.WhatsApp;

    public async Task<SendResult> SendAsync(SendRequest request, CancellationToken ct = default)
    {
        var url = $"{_opt.ApiBaseUrl}/{_opt.PhoneNumberId}/messages";

        // Cloud API "text" message payload.
        var payload = new
        {
            messaging_product = "whatsapp",
            to = request.Recipient,          // E.164 without '+', per Cloud API
            type = "text",
            text = new { body = request.Body }
        };

        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };
            req.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _opt.AccessToken);

            using var res = await _http.SendAsync(req, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("WhatsApp send failed ({Status}): {Body}", res.StatusCode, json);
                return SendResult.Fail($"WhatsApp API {(int)res.StatusCode}: {Truncate(json)}");
            }

            // Response: { "messages": [ { "id": "wamid...." } ] }
            var wamid = ExtractMessageId(json);
            return wamid is not null
                ? SendResult.Ok(wamid)
                : SendResult.Fail("WhatsApp API returned no message id.");
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "WhatsApp send threw for {Recipient}", request.Recipient);
            return SendResult.Fail($"WhatsApp error: {ex.Message}");
        }
    }

    private static string? ExtractMessageId(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("messages")[0]
                .GetProperty("id")
                .GetString();
        }
        catch { return null; }
    }

    private static string Truncate(string s) => s.Length > 200 ? s[..200] : s;
}
