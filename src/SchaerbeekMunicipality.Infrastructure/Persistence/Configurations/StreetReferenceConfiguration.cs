using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class StreetReferenceConfiguration : IEntityTypeConfiguration<StreetReference>
{
    public void Configure(EntityTypeBuilder<StreetReference> builder)
    {
        builder.ToTable("streets");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(4);

        builder.HasIndex(s => s.PostalCode);

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(128);
    }
}
