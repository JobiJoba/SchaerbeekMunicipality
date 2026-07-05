using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class OutboundNotificationConfiguration : IEntityTypeConfiguration<OutboundNotification>
{
    public void Configure(EntityTypeBuilder<OutboundNotification> builder)
    {
        builder.ToTable("outbound_notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasConversion(
                id => id.Value,
                value => OutboundNotificationId.From(value))
            .HasColumnName("id");

        builder.Property(n => n.RegistrationCaseId)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("registration_case_id");

        builder.Property(n => n.PersonId)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
            .HasColumnName("person_id");

        builder.Property(n => n.Recipient)
            .HasColumnName("recipient")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(n => n.Message)
            .HasColumnName("message")
            .HasMaxLength(500);

        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(n => n.RegistrationCaseId);
        builder.HasIndex(n => n.CreatedAt);
    }
}
