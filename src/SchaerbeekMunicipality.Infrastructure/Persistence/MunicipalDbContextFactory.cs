using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchaerbeekMunicipality.Infrastructure.Persistence;

/// <summary>
/// Ensures <c>dotnet ef</c> generates PostgreSQL migrations (snake_case, Npgsql types)
/// even when the startup project's default connection string targets SQLite.
/// </summary>
public sealed class MunicipalDbContextFactory : IDesignTimeDbContextFactory<MunicipalDbContext>
{
    public MunicipalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MunicipalDbContext>();
        optionsBuilder
            .UseNpgsql("Host=localhost;Database=schaerbeek;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention();

        return new MunicipalDbContext(optionsBuilder.Options);
    }
}
