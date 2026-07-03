using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class RegistrationCaseConfiguration : IEntityTypeConfiguration<RegistrationCase>
{
    public void Configure(EntityTypeBuilder<RegistrationCase> builder)
    {
        builder.ToTable("registration_cases");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new RegistrationCaseId(value))
            .HasColumnName("id");

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64);
    }
}
