using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> b)
    {
        b.ToTable("companies");
        b.HasKey(c => c.Id);

        b.Property(c => c.Name).IsRequired().HasMaxLength(256);
        b.Property(c => c.Industry).HasMaxLength(120);
        b.Property(c => c.Website).HasMaxLength(256);
        b.Property(c => c.Phone).HasMaxLength(40);
        b.Property(c => c.Email).HasMaxLength(256);
        b.Property(c => c.AddressCity).HasMaxLength(120);
        b.Property(c => c.AddressCountry).HasMaxLength(120);

        // Money as numeric — never floating point.
        b.Property(c => c.AnnualRevenue).HasColumnType("numeric(18,2)");

        b.HasOne(c => c.AssignedTo)
            .WithMany(u => u.AssignedCompanies)
            .HasForeignKey(c => c.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(c => c.Name);
        b.HasIndex(c => c.AssignedToId);

        b.HasQueryFilter(c => !c.IsDeleted);
    }
}
