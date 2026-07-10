using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class PoliceVerificationRequestConfiguration : IEntityTypeConfiguration<PoliceVerificationRequest>
{
    public void Configure(EntityTypeBuilder<PoliceVerificationRequest> builder)
    {
        builder.ToTable("police_verification_requests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => PoliceVerificationRequestId.From(value))
            .HasColumnName("id");

        builder.Property(r => r.RegistrationCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new RegistrationCaseId(value.Value) : null)
            .HasColumnName("registration_case_id");

        builder.Property(r => r.ChangeOfAddressCaseId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new ChangeOfAddressCaseId(value.Value) : null)
            .HasColumnName("change_of_address_case_id");

        builder.Property(r => r.RequestedAt)
            .HasColumnName("requested_at");

        builder.Property(r => r.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(r => r.Result)
            .HasColumnName("result")
            .HasConversion<string>()
            .HasMaxLength(64);

        builder.Property(r => r.OfficerNotes)
            .HasColumnName("officer_notes")
            .HasMaxLength(2000);

        builder.Property(r => r.AttemptNumber)
            .HasColumnName("attempt_number");

        builder.HasIndex(r => r.RegistrationCaseId);
        builder.HasIndex(r => r.ChangeOfAddressCaseId);

        builder.HasIndex(r => r.CompletedAt);
    }
}
