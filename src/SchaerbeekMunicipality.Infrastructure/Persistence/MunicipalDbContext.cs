using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.ReferenceData;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

public sealed class MunicipalDbContext(DbContextOptions<MunicipalDbContext> options) : DbContext(options)
{
    public DbSet<RegistrationCase> RegistrationCases => Set<RegistrationCase>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<AdministrativeDocument> AdministrativeDocuments => Set<AdministrativeDocument>();

    public DbSet<ResidencePermit> ResidencePermits => Set<ResidencePermit>();

    public DbSet<Household> Households => Set<Household>();

    public DbSet<HouseholdMember> HouseholdMembers => Set<HouseholdMember>();

    public DbSet<MunicipalityReference> Municipalities => Set<MunicipalityReference>();

    public DbSet<StreetReference> Streets => Set<StreetReference>();

    public DbSet<NationalRegisterPerson> NationalRegisterPersons => Set<NationalRegisterPerson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MunicipalDbContext).Assembly);
    }
}
