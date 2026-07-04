using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class MunicipalityReferenceConfiguration : IEntityTypeConfiguration<MunicipalityReference>
{
    public void Configure(EntityTypeBuilder<MunicipalityReference> builder)
    {
        builder.ToTable("municipalities");

        builder.HasKey(m => m.PostalCode);

        builder.Property(m => m.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(4);

        builder.Property(m => m.Name)
            .HasColumnName("name")
            .HasMaxLength(128);
    }
}
