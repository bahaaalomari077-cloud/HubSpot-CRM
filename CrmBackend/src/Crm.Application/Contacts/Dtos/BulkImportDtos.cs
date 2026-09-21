using System.ComponentModel.DataAnnotations;
using Crm.Domain.Enums;

namespace Crm.Application.Contacts.Dtos;

/// <summary>One parsed line from the paste box.</summary>
public class BulkImportRowDto
{
    [Required]
    [StringLength(240, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(40)]
    public string? Phone { get; set; }
}

/// <summary>A pasted batch. Rows are parsed client-side, validated here.</summary>
public class BulkImportDto
{
    [MinLength(1, ErrorMessage = "Nothing to import.")]
    [MaxLength(500, ErrorMessage = "Import at most 500 rows at a time.")]
    public List<BulkImportRowDto> Rows { get; set; } = new();

    /// <summary>Everything lands here. Defaults to Potential ("suggested").</summary>
    [EnumDataType(typeof(ContactStatus))]
    public ContactStatus Status { get; set; } = ContactStatus.Potential;
}

/// <summary>What happened, per row and in total.</summary>
public class BulkImportResultDto
{
    public int Created { get; set; }
    public int Skipped { get; set; }

    /// <summary>Human-readable reasons for the rows that did not import.</summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>The contacts actually created, so the board can show them at once.</summary>
    public List<ContactDto> Contacts { get; set; } = new();
}
