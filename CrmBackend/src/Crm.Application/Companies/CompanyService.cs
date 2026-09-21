using Crm.Application.Common.Interfaces;
using Crm.Application.Common.Models;
using Crm.Application.Companies.Dtos;
using Crm.Domain.Entities;
using Crm.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Crm.Application.Companies;

public interface ICompanyService
{
    Task<PagedResult<CompanyDto>> GetPagedAsync(CompanyQueryParameters query, CancellationToken ct = default);
    Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CompanyDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken ct = default);
    Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public class CompanyService : ICompanyService
{
    private readonly ICrmDbContext _db;
    private readonly ILogger<CompanyService> _log;

    public CompanyService(ICrmDbContext db, ILogger<CompanyService> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<PagedResult<CompanyDto>> GetPagedAsync(CompanyQueryParameters query, CancellationToken ct = default)
    {
        var q = _db.Companies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(c =>
                c.Name.ToLower().Contains(term) ||
                (c.Industry != null && c.Industry.ToLower().Contains(term)) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(term)) ||
                (c.AddressCity != null && c.AddressCity.ToLower().Contains(term)));
        }

        q = (query.SortBy?.ToLowerInvariant(), query.SortDescending) switch
        {
            ("name", false)     => q.OrderBy(c => c.Name),
            ("name", true)      => q.OrderByDescending(c => c.Name),
            ("industry", false) => q.OrderBy(c => c.Industry),
            ("industry", true)  => q.OrderByDescending(c => c.Industry),
            (_, true)           => q.OrderByDescending(c => c.CreatedAt),
            _                   => q.OrderBy(c => c.Name)
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Industry = c.Industry,
                Website = c.Website,
                Phone = c.Phone,
                Email = c.Email,
                AddressCity = c.AddressCity,
                AddressCountry = c.AddressCountry,
                AnnualRevenue = c.AnnualRevenue,
                EmployeeCount = c.EmployeeCount,
                ContactCount = c.Contacts.Count(x => !x.IsDeleted),
                ActiveContacts = c.Contacts.Count(x => !x.IsDeleted &&
                    (x.Status == ContactStatus.Interested ||
                     x.Status == ContactStatus.Applied ||
                     x.Status == ContactStatus.Client)),
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<CompanyDto>(items, query.Page, query.PageSize, total);
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _db.Companies.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Industry = c.Industry,
                Website = c.Website,
                Phone = c.Phone,
                Email = c.Email,
                AddressCity = c.AddressCity,
                AddressCountry = c.AddressCountry,
                AnnualRevenue = c.AnnualRevenue,
                EmployeeCount = c.EmployeeCount,
                ContactCount = c.Contacts.Count(x => !x.IsDeleted),
                ActiveContacts = c.Contacts.Count(x => !x.IsDeleted &&
                    (x.Status == ContactStatus.Interested ||
                     x.Status == ContactStatus.Applied ||
                     x.Status == ContactStatus.Client)),
                CreatedAt = c.CreatedAt
            })
            .FirstOrDefaultAsync(ct);

        return company;
    }

    /// <summary>Company + its contacts + a status breakdown, in one round trip pair.</summary>
    public async Task<CompanyDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        var company = await GetByIdAsync(id, ct);
        if (company is null) return null;

        var contacts = await _db.Contacts.AsNoTracking()
            .Where(x => x.CompanyId == id)
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Select(x => new CompanyContactDto
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Position = x.Position,
                Status = x.Status.ToString(),
                NextAction = x.NextAction,
                NextActionDate = x.NextActionDate
            })
            .ToListAsync(ct);

        // Group in memory — the list is already loaded and small per company.
        var byStatus = Enum.GetValues<ContactStatus>()
            .Select(st => new CompanyStatusCountDto
            {
                Status = st.ToString(),
                Count = contacts.Count(c => c.Status == st.ToString())
            })
            .Where(g => g.Count > 0)
            .ToList();

        var active = contacts.Count(c =>
            c.Status == nameof(ContactStatus.Interested) ||
            c.Status == nameof(ContactStatus.Applied) ||
            c.Status == nameof(ContactStatus.Client));

        return new CompanyDetailsDto
        {
            Company = company,
            Contacts = contacts,
            ContactsByStatus = byStatus,
            ActiveContacts = active,
            IsActive = active > 0
        };
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto, CancellationToken ct = default)
    {
        var company = new Company
        {
            Name = dto.Name,
            Industry = dto.Industry,
            Website = dto.Website,
            Phone = dto.Phone,
            Email = dto.Email,
            AddressCity = dto.AddressCity,
            AddressCountry = dto.AddressCountry,
            AnnualRevenue = dto.AnnualRevenue,
            EmployeeCount = dto.EmployeeCount
        };

        _db.Companies.Add(company);
        await _db.SaveChangesAsync(ct);

        _log.LogInformation("Company created {CompanyId} '{Name}'", company.Id, company.Name);

        return Map(company);
    }

    public async Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto, CancellationToken ct = default)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (company is null) return null;

        company.Name = dto.Name;
        company.Industry = dto.Industry;
        company.Website = dto.Website;
        company.Phone = dto.Phone;
        company.Email = dto.Email;
        company.AddressCity = dto.AddressCity;
        company.AddressCountry = dto.AddressCountry;
        company.AnnualRevenue = dto.AnnualRevenue;
        company.EmployeeCount = dto.EmployeeCount;
        company.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Map(company);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (company is null) return false;

        company.IsDeleted = true;
        company.DeletedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static CompanyDto Map(Company c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Industry = c.Industry,
        Website = c.Website,
        Phone = c.Phone,
        Email = c.Email,
        AddressCity = c.AddressCity,
        AddressCountry = c.AddressCountry,
        AnnualRevenue = c.AnnualRevenue,
        EmployeeCount = c.EmployeeCount,
        CreatedAt = c.CreatedAt
    };
}
