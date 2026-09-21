using Crm.Application.Activities;
using Crm.Application.Applications;
using Crm.Application.Companies;
using Crm.Application.Contacts;
using Crm.Application.Dashboard;
using Crm.Application.Messaging;
using Crm.Application.Timeline;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<ITimelineService, TimelineService>();
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICompanyService, CompanyService>();
        // Users service registers here when added.
        return services;
    }
}
