namespace Crm.Domain.Enums;

/// <summary>
/// Where a contact stands with us. Set manually by the admin — no workflow engine,
/// no transition rules. Any status can move to any other.
/// </summary>
public enum ContactStatus
{
    Potential = 0,   // suggested, no contact yet
    Contacted = 1,   // message sent, waiting response
    Interested = 2,
    Applied = 3,
    Client = 4,      // joined
    Rejected = 5
}

/// <summary>Pipeline semantic — copied from Bitrix24's P/S/F stage-group model.</summary>
public enum StageSemantic
{
    Process = 0,   // in progress
    Success = 1,   // won / converted
    Failure = 2    // lost / junk
}

/// <summary>Stage of an Application (the deal/pipeline entity in this CRM).</summary>
public enum ApplicationStage
{
    New = 0,
    CreatePapers = 1,
    Invoice = 2,
    InProgress = 3,
    FinalInvoice = 4,
    Won = 5,
    Lost = 6
}

/// <summary>Activity type — mirrors Bitrix24 crm.enum.activitytype.</summary>
public enum ActivityType
{
    Meeting = 1,
    Call = 2,
    Task = 3,
    Email = 4,
    General = 5,
    UserAction = 6,
    Note = 7
}

public enum ActivityStatus
{
    Pending = 1,
    Completed = 2,
    CompletedAutomatically = 3
}

/// <summary>Channel a Message came through — a subset of Bitrix24's Contact Center.</summary>
public enum MessageChannel
{
    Email = 0,
    Sms = 1,
    WhatsApp = 2,
    Telegram = 3,
    LiveChat = 4,
    Call = 5
}

public enum MessageDirection
{
    Inbound = 0,
    Outbound = 1
}

/// <summary>Delivery lifecycle of an outbound message.</summary>
public enum MessageStatus
{
    Draft = 0,      // created, not yet dispatched
    Queued = 1,     // accepted for sending
    Sent = 2,       // handed off to the provider successfully
    Delivered = 3,  // provider confirmed delivery (webhook)
    Failed = 4,     // send attempt failed
    Received = 5    // inbound message (no send lifecycle)
}

public enum UserRole
{
    Admin = 0,
    Manager = 1,
    Agent = 2
}
