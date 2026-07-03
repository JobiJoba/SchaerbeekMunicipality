using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

public sealed class MunicipalDbContext(DbContextOptions<MunicipalDbContext> options) : DbContext(options)
{
    public DbSet<RegistrationCase> RegistrationCases => Set<RegistrationCase>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MunicipalDbContext).Assembly);
    }
}
