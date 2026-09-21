using Crm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crm.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.ToTable("messages");
        b.HasKey(m => m.Id);

        b.Property(m => m.Channel).HasConversion<int>();
        b.Property(m => m.Direction).HasConversion<int>();
        b.Property(m => m.Status).HasConversion<int>();
        b.Property(m => m.Subject).HasMaxLength(256);
        b.Property(m => m.Body).IsRequired().HasColumnType("text");
        b.Property(m => m.Recipient).HasMaxLength(320);          // max email length
        b.Property(m => m.ProviderMessageId).HasMaxLength(256);
        b.Property(m => m.FailureReason).HasMaxLength(1000);

        // Message → Contact (required). Cascade: messages belong to their contact.
        b.HasOne(m => m.Contact)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message → User (nullable — inbound machine messages have no user).
        b.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasIndex(m => new { m.ContactId, m.SentAt });
        b.HasIndex(m => m.Channel);
        b.HasIndex(m => m.Status);                  // for retry sweeps of failed/queued
        b.HasIndex(m => m.ProviderMessageId);       // webhook status correlation

        b.HasQueryFilter(m => !m.IsDeleted);
    }
}
