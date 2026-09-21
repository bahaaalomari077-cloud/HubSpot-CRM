using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> b)
    {
        b.ToTable("activities");
        b.HasKey(a => a.Id);

        b.Property(a => a.Subject).IsRequired().HasMaxLength(256);
        b.Property(a => a.Description).HasColumnType("text");
        b.Property(a => a.Type).HasConversion<int>();
        b.Property(a => a.Status).HasConversion<int>();

        b.HasOne(a => a.Contact)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.ContactId)
            .OnDelete(DeleteBehavior.Cascade); // activities die with their contact

        b.HasOne(a => a.Application)
            .WithMany(app => app.Activities)
            .HasForeignKey(a => a.ApplicationId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(a => a.AssignedTo)
            .WithMany(u => u.AssignedActivities)
            .HasForeignKey(a => a.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        // Powers the "overdue" and "due today" counters.
        b.HasIndex(a => new { a.AssignedToId, a.Deadline, a.IsCompleted });

        // Timeline query: filter by ContactId, sort by CreatedAt DESC, Take(50).
        // The composite lets Postgres satisfy the WHERE *and* the ORDER BY from one
        // index scan — no sort step, and it stops after 50 rows.
        b.HasIndex(a => new { a.ContactId, a.CreatedAt });
        b.HasIndex(a => a.ApplicationId);

        b.HasQueryFilter(a => !a.IsDeleted);
    }
}
