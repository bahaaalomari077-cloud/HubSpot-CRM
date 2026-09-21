namespace Crm.Infrastructure.Messaging;

/// <summary>SMTP settings, bound from configuration section "Messaging:Smtp".</summary>
public class SmtpOptions
{
    public const string Section = "Messaging:Smtp";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public bool UseStartTls { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromName { get; set; } = "CRM";
    public string FromAddress { get; set; } = "no-reply@example.com";
}

/// <summary>WhatsApp Cloud API settings, bound from "Messaging:WhatsApp".</summary>
public class WhatsAppOptions
{
    public const string Section = "Messaging:WhatsApp";

    /// <summary>Meta Graph API base, e.g. https://graph.facebook.com/v21.0</summary>
    public string ApiBaseUrl { get; set; } = "https://graph.facebook.com/v21.0";

    /// <summary>The WhatsApp Business phone-number id.</summary>
    public string PhoneNumberId { get; set; } = string.Empty;

    /// <summary>Permanent access token (store in user-secrets / key vault, never appsettings).</summary>
    public string AccessToken { get; set; } = string.Empty;
}
