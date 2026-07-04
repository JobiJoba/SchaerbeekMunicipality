using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchaerbeekMunicipality.Domain.Household;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Configurations;

internal sealed class HouseholdMemberConfiguration : IEntityTypeConfiguration<HouseholdMember>
{
    public void Configure(EntityTypeBuilder<HouseholdMember> builder)
    {
        builder.ToTable("household_members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => HouseholdMemberId.From(value))
            .HasColumnName("id");

        builder.Property(m => m.HouseholdId)
            .HasConversion(
                id => id.Value,
                value => HouseholdId.From(value))
            .HasColumnName("household_id");

        builder.Property(m => m.GivenName)
            .HasColumnName("given_name")
            .HasMaxLength(128);

        builder.Property(m => m.FamilyName)
            .HasColumnName("family_name")
            .HasMaxLength(128);

        builder.Property(m => m.BirthDate)
            .HasColumnName("birth_date");

        builder.Property(m => m.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasMaxLength(32);
    }
}
