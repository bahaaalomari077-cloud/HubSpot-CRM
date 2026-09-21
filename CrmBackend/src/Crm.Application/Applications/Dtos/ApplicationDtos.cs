using Crm.Domain.Enums;

namespace Crm.Application.Applications.Dtos;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ApplicationStage Stage { get; set; }
    public StageSemantic StageSemantic { get; set; }
    public bool IsClosed { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Probability { get; set; }
    public DateTimeOffset? BeginDate { get; set; }
    public DateTimeOffset? CloseDate { get; set; }
    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateApplicationDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Probability { get; set; }
    public ApplicationStage Stage { get; set; } = ApplicationStage.New;
    public Guid? ContactId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? AssignedToId { get; set; }
}

public class UpdateApplicationDto
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Probability { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? AssignedToId { get; set; }
}

/// <summary>Dedicated stage-move payload — mirrors Bitrix24 kanban drag semantics.</summary>
public class MoveStageDto
{
    public ApplicationStage Stage { get; set; }
}
