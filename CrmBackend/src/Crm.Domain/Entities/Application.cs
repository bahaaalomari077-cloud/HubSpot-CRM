using Crm.Domain.Common;
using Crm.Domain.Enums;

namespace Crm.Domain.Entities;

/// <summary>
/// The deal/pipeline entity. Money is <c>decimal</c>, never double — the one
/// place the Bitrix24 reference schema is deliberately not copied, since
/// floating point drifts on repeated pipeline sums.
/// </summary>
public class Application : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public ApplicationStage Stage { get; set; } = ApplicationStage.New;
    public StageSemantic StageSemantic { get; set; } = StageSemantic.Process;
    public bool IsClosed { get; set; }

    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public int Probability { get; set; }

    public DateTimeOffset? BeginDate { get; set; }
    public DateTimeOffset? CloseDate { get; set; }

    // Relationships — an application is anchored to a contact and/or a company.
    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public Guid? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
