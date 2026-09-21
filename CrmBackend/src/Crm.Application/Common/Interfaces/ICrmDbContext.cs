using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = Crm.Domain.Entities.Application;

namespace Crm.Application.Common.Interfaces;

/// <summary>
/// Abstraction the Application layer depends on, so it never references the
/// concrete Infrastructure DbContext (Clean Architecture dependency rule).
/// </summary>
public interface ICrmDbContext
{
    DbSet<Contact> Contacts { get; }
    DbSet<Company> Companies { get; }
    DbSet<ApplicationEntity> Applications { get; }
    DbSet<Activity> Activities { get; }
    DbSet<Message> Messages { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
