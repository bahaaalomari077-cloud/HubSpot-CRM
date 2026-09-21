using System.ComponentModel.DataAnnotations;

namespace Crm.Application.Companies.Dtos;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressCountry { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public int? EmployeeCount { get; set; }

    /// <summary>How many contacts belong to this company.</summary>
    public int ContactCount { get; set; }

    /// <summary>Contacts in Interested / Applied / Client — drives the "active" highlight.</summary>
    public int ActiveContacts { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}

public class CreateCompanyDto
{
    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(256, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Industry { get; set; }

    [StringLength(256)]
    public string? Website { get; set; }

    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "Phone must be 7-15 digits, optionally starting with '+'.")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(120)]
    public string? AddressCity { get; set; }

    [StringLength(120)]
    public string? AddressCountry { get; set; }

    [Range(0, 999_999_999_999)]
    public decimal? AnnualRevenue { get; set; }

    [Range(0, 10_000_000)]
    public int? EmployeeCount { get; set; }
}

public class UpdateCompanyDto : CreateCompanyDto { }

/// <summary>Contacts grouped by status, for a company's insight strip.</summary>
public class CompanyStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>A contact row shown under a company.</summary>
public class CompanyContactDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? NextAction { get; set; }
    public DateTimeOffset? NextActionDate { get; set; }
}

/// <summary>Everything the company details page needs, in one call.</summary>
public class CompanyDetailsDto
{
    public CompanyDto Company { get; set; } = new();
    public List<CompanyContactDto> Contacts { get; set; } = new();
    public List<CompanyStatusCountDto> ContactsByStatus { get; set; } = new();

    /// <summary>Contacts in Interested / Applied / Client — the ones worth attention.</summary>
    public int ActiveContacts { get; set; }

    /// <summary>True when this company has at least one active contact.</summary>
    public bool IsActive { get; set; }
}
