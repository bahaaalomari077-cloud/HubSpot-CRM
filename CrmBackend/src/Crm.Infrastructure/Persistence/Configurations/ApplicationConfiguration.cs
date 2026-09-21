using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = Crm.Domain.Entities.Application;

namespace Crm.Infrastructure.Persistence.Configurations;

public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> b)
    {
        b.ToTable("applications");
        b.HasKey(a => a.Id);

        b.Property(a => a.Title).IsRequired().HasMaxLength(256);

        // Enums stored as int (compact, index-friendly).
        b.Property(a => a.Stage).HasConversion<int>();
        b.Property(a => a.StageSemantic).HasConversion<int>();

        // Money as numeric(18,2) — the deliberate divergence from Bitrix24's double.
        b.Property(a => a.Amount).HasColumnType("numeric(18,2)");
        b.Property(a => a.CurrencyCode).IsRequired().HasMaxLength(3).IsFixedLength();

        // Application → Contact (many-to-one). SetNull keeps the deal if the contact goes.
        b.HasOne(a => a.Contact)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        // Application → Company (many-to-one).
        b.HasOne(a => a.Company)
            .WithMany(co => co.Applications)
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Application → responsible User.
        b.HasOne(a => a.AssignedTo)
            .WithMany(u => u.AssignedApplications)
            .HasForeignKey(a => a.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite index that the kanban board query hits.
        b.HasIndex(a => new { a.Stage, a.IsClosed });
        b.HasIndex(a => a.CompanyId);
        b.HasIndex(a => a.ContactId);
        b.HasIndex(a => a.AssignedToId);

        b.HasQueryFilter(a => !a.IsDeleted);
    }
}
