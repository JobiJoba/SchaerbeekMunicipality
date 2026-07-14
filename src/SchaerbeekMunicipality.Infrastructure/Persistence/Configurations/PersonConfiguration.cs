using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

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

        builder.Property(p => p.BirthPlace)
            .HasColumnName("birth_place")
            .HasMaxLength(128);

        builder.Property(p => p.BirthCountry)
            .HasColumnName("birth_country")
            .HasMaxLength(64);

        builder.Property(p => p.LinkedRegisterRecordId)
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? NationalRegisterPersonId.From(value.Value) : null)
            .HasColumnName("linked_register_record_id");

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

        builder.HasIndex(p => p.LinkedRegisterRecordId);
        builder.HasIndex(p => p.NationalRegisterNumber).IsUnique();

        builder.OwnsOne(p => p.DomicileAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("domicile_street").HasMaxLength(256);
            address.Property(a => a.HouseNumber).HasColumnName("domicile_house_number").HasMaxLength(16);
            address.Property(a => a.Box).HasColumnName("domicile_box").HasMaxLength(16);
            address.Property(a => a.PostalCode).HasColumnName("domicile_postal_code").HasMaxLength(4);
            address.Property(a => a.Municipality).HasColumnName("domicile_municipality").HasMaxLength(128);
        });

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

            civilStatus.Property(c => c.MarriageRecognitionStatus)
                .HasColumnName("marriage_recognition_status")
                .HasConversion<string>()
                .HasMaxLength(32);
        });
    }
}