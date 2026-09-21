using Crm.Application.Common.Interfaces;
using Crm.Application.Messaging.Providers;
using Crm.Infrastructure.Messaging;
using Crm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not configured.");

        services.AddDbContext<CrmDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(CrmDbContext).Assembly.FullName)));

        // Expose the context through its Application-layer interface.
        services.AddScoped<ICrmDbContext>(sp => sp.GetRequiredService<CrmDbContext>());

        // ---- Messaging integration ----
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.Section));
        services.Configure<WhatsAppOptions>(configuration.GetSection(WhatsAppOptions.Section));

        // Email provider (SMTP).
        services.AddScoped<IMessageProvider, SmtpEmailProvider>();

        // WhatsApp provider gets a typed HttpClient (pooled, resilient).
        services.AddHttpClient<WhatsAppProvider>();
        services.AddScoped<IMessageProvider>(sp => sp.GetRequiredService<WhatsAppProvider>());

        services.AddScoped<IMessageProviderFactory, MessageProviderFactory>();

        return services;
    }
}
