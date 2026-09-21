using System.ComponentModel.DataAnnotations;

namespace Crm.Application.Common.Validation;

/// <summary>
/// Rejects Guid.Empty. Needed because [Required] never fires on a value type —
/// a missing Guid in JSON silently becomes 00000000-0000-....
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value switch
    {
        null => true,                      // nullable Guid? left unset is fine
        Guid g => g != Guid.Empty,
        _ => false
    };
}
