namespace Crm.Application.Common.Models;

/// <summary>Base paging + search parameters bound from the query string on every list endpoint.</summary>
public abstract class QueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    private int _page = 1;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > MaxPageSize ? 20 : value;
    }

    /// <summary>Free-text search term applied per-endpoint.</summary>
    public string? Search { get; set; }

    /// <summary>Field name to sort by (endpoint validates against an allow-list).</summary>
    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }
}

public class ContactQueryParameters : QueryParameters
{
    public Guid? CompanyId { get; set; }
    public Guid? AssignedToId { get; set; }
}

public class ApplicationQueryParameters : QueryParameters
{
    public int? Stage { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? AssignedToId { get; set; }
    public bool? IsClosed { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class ActivityQueryParameters : QueryParameters
{
    public int? Type { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? AssignedToId { get; set; }
    public bool? IsCompleted { get; set; }
}
