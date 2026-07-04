using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PersonId(value))
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

        builder.OwnsOne(p => p.CivilStatus, civilStatus =>
        {
            civilStatus.Property(c => c.Status)
                .HasColumnName("civil_status")
                .HasConversion<string>()
                .HasMaxLength(32);

            civilStatus.Property(c => c.SpouseGivenName)
                .HasColumnName("spouse_given_name")
                .HasMaxLength(128);

            civilStatus.Property(c => c.SpouseFamilyName)
                .HasColumnName("spouse_family_name")
                .HasMaxLength(128);

            civilStatus.Property(c => c.MarriageDate)
                .HasColumnName("marriage_date");

            civilStatus.Property(c => c.MarriagePlace)
                .HasColumnName("marriage_place")
                .HasMaxLength(128);
        });
    }
}
