using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

public sealed class MunicipalDbContext(DbContextOptions<MunicipalDbContext> options) : DbContext(options)
{
    public DbSet<RegistrationCase> RegistrationCases => Set<RegistrationCase>();

    public DbSet<Person> Persons => Set<Person>();

    public DbSet<AdministrativeDocument> AdministrativeDocuments => Set<AdministrativeDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MunicipalDbContext).Assembly);
    }
}
