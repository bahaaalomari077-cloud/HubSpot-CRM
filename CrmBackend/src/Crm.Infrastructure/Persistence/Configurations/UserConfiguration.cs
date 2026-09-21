using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(u => u.Id);

        b.Property(u => u.FirstName).IsRequired().HasMaxLength(120);
        b.Property(u => u.LastName).IsRequired().HasMaxLength(120);
        b.Property(u => u.Email).IsRequired().HasMaxLength(256);
        b.Property(u => u.PasswordHash).IsRequired();
        b.Property(u => u.Role).HasConversion<int>();
        b.Ignore(u => u.FullName);

        // Email is the login identity — unique.
        b.HasIndex(u => u.Email).IsUnique();

        b.HasQueryFilter(u => !u.IsDeleted);
    }
}
