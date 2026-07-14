using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class NationalRegisterPersonConfiguration : IEntityTypeConfiguration<NationalRegisterPerson>
{
    public void Configure(EntityTypeBuilder<NationalRegisterPerson> builder)
    {
        builder.ToTable("national_register_persons");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => NationalRegisterPersonId.From(value))
            .HasColumnName("id");

        builder.Property(p => p.GivenName)
            .HasColumnName("given_name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(p => p.FamilyName)
            .HasColumnName("family_name")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(p => p.BirthDate)
            .HasColumnName("birth_date");

        builder.Property(p => p.Nationality)
            .HasColumnName("nationality")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(p => p.BisNumber)
            .HasConversion(
                number => number.HasValue ? number.Value.Value : null,
                value => value == null ? null : BisNumber.Create(value))
            .HasColumnName("bis_number")
            .HasMaxLength(11);

        builder.Property(p => p.NationalRegisterNumber)
            .HasConversion(
                number => number.HasValue ? number.Value.Value : null,
                value => value == null ? null : NationalRegisterNumber.Create(value))
            .HasColumnName("national_register_number")
            .HasMaxLength(11);

        builder.HasIndex(p => p.BisNumber).IsUnique();
        builder.HasIndex(p => p.NationalRegisterNumber).IsUnique();
    }
}