# Messaging Integration — Email (SMTP) + WhatsApp

Backend service design for sending outbound messages over multiple channels,
storing every message, and tracking delivery status. Built into the CRM backend
and **verified live** against real SMTP + the Meta Graph API.

## Status

- ✅ Builds clean · migration `AddMessageStatusTracking` applied to `HubS-pot-CRM`
- ✅ Live-tested: Email hit a real SMTP socket, WhatsApp hit the real Graph API;
  both failures were caught, returned 200 with `status: Failed`, and **persisted**
  with the actual provider error, `AttemptCount`, and `LastAttemptAt`.

## The design in one line

`SendAsync` → **persist as Queued** → resolve provider by channel → **dispatch** →
**record Sent / Failed** on the same row. Nothing is ever sent without first being
stored, and no expected failure ever throws.

## Provider abstraction

```
IMessageProvider            (one per channel)
  ├─ MessageChannel Channel
  └─ Task<SendResult> SendAsync(SendRequest)

SendResult = Success | (ProviderMessageId) | (FailureReason)   -- never throws

IMessageProviderFactory.For(channel) -> IMessageProvider
```

Two implementations live in **Infrastructure**:

| Provider | Channel | Transport |
|---|---|---|
| `SmtpEmailProvider` | Email | MailKit `SmtpClient` (STARTTLS, auth) |
| `WhatsAppProvider` | WhatsApp | typed `HttpClient` → Meta WhatsApp Cloud API |

Adding a channel (SMS, Telegram, …) = one new `IMessageProvider` + one DI line.
The factory and service don't change.

## Status lifecycle

```
Draft → Queued → Sent → Delivered        (Delivered set later via webhook)
              └────────→ Failed           (reason stored, retryable)
Inbound messages: Received
```

Tracked columns on `Message`:

| Column | Purpose |
|---|---|
| `Status` | Draft / Queued / Sent / Delivered / Failed / Received |
| `Recipient` | email or E.164 phone the message went to |
| `ProviderMessageId` | SMTP Message-Id / WhatsApp `wamid` — for webhook correlation |
| `FailureReason` | provider error text when a send fails |
| `AttemptCount` | number of dispatch attempts (enables retry) |
| `LastAttemptAt` | when the last attempt ran |

Indexes: `Status` (retry sweeps of Queued/Failed) and `ProviderMessageId`
(delivery-webhook correlation).

## Why persist before sending

The message row is written as **Queued before dispatch**. If the process crashes
mid-send, the record survives with `Status = Queued`, so a background sweep can find
and retry it (`WHERE Status IN (Queued, Failed) AND AttemptCount < N`). This is the
outbox pattern — the database is the source of truth, not the in-flight HTTP call.

## API

```
POST /api/messages/send
{
  "contactId": "…",
  "channel": 0,                 // 0 Email · 2 WhatsApp
  "recipient": "someone@x.com", // or "9627xxxxxxxx" for WhatsApp
  "subject": "Welcome",         // email only
  "body": "Hello from CRM"
}
→ 200 { messageId, status, providerMessageId, failureReason }
```

Returns **200 even on provider failure** — the attempt is recorded and `status`
tells the caller what happened. A `4xx` is reserved for a malformed request.

## Configuration (never in appsettings — use user-secrets / key vault)

```
Messaging:Smtp:Host, Port, UseStartTls, Username, Password, FromName, FromAddress
Messaging:WhatsApp:ApiBaseUrl, PhoneNumberId, AccessToken
```

## Files

```
Crm.Application/Messaging/
  Providers/IMessageProvider.cs         SendRequest, SendResult, IMessageProvider
  Providers/IMessageProviderFactory.cs
  Dtos/MessagingDtos.cs                 SendMessageDto, SendMessageResultDto
  IMessagingService.cs
  MessagingService.cs                   persist → dispatch → record status
Crm.Infrastructure/Messaging/
  MessagingOptions.cs                   SmtpOptions, WhatsAppOptions
  SmtpEmailProvider.cs                  MailKit
  WhatsAppProvider.cs                   Meta Cloud API
  MessageProviderFactory.cs
Crm.Api/Controllers/MessagesController.cs   POST /api/messages/send
```

## Next steps (not built)

- **Delivery webhooks** — Meta/ESP callbacks flip `Sent → Delivered/Failed` by
  matching `ProviderMessageId`. The index is already in place.
- **Retry worker** — a `BackgroundService` (or BullMQ-style queue) that re-dispatches
  `Queued`/`Failed` rows with capped `AttemptCount` and exponential backoff.
- **Rate limiting** per provider (WhatsApp has strict tiers).
```
