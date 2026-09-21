using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Crm.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations` / `database update` can build the
/// context without booting the API. Reads the same connection string the API uses.
/// </summary>
public class CrmDbContextFactory : IDesignTimeDbContextFactory<CrmDbContext>
{
    public CrmDbContext CreateDbContext(string[] args)
    {
        // Look for the API's appsettings.json relative to the Infrastructure project.
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Crm.Api");
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            basePath = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets(typeof(CrmDbContextFactory).Assembly, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("Default")
            ?? "Host=localhost;Port=5433;Database=HubS-pot-CRM;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<CrmDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CrmDbContext(options);
    }
}
