using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> b)
    {
        b.ToTable("contacts");
        b.HasKey(c => c.Id);

        b.Property(c => c.FirstName).IsRequired().HasMaxLength(120);
        b.Property(c => c.LastName).IsRequired().HasMaxLength(120);
        b.Property(c => c.Email).HasMaxLength(256);
        b.Property(c => c.Phone).HasMaxLength(40);
        b.Property(c => c.Position).HasMaxLength(160);
        b.Ignore(c => c.FullName);

        // Contact → Company (many contacts to one company). Nullable FK,
        // SetNull on delete so removing a company orphans, not deletes, its contacts.
        b.HasOne(c => c.Company)
            .WithMany(co => co.Contacts)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Contact → responsible User. Restrict: a user with assigned records can't be hard-deleted.
        b.HasOne(c => c.AssignedTo)
            .WithMany(u => u.AssignedContacts)
            .HasForeignKey(c => c.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(c => c.CompanyId);
        b.HasIndex(c => c.AssignedToId);
        b.HasIndex(c => c.Email);

        // Global soft-delete filter — hides deleted rows from every query.
        b.HasQueryFilter(c => !c.IsDeleted);
    }
}
