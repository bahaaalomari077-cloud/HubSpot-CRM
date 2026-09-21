namespace Crm.Domain.Common;

/// <summary>
/// Shared base for every persisted entity. Mirrors the audit columns that
/// Bitrix24 stamps on all of its CRM entities (created/updated by + timestamps),
/// plus soft-delete so records are never physically removed.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedById { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }

    // Soft delete — filtered out globally in the DbContext.
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
