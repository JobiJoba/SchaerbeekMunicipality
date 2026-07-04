using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace SchaerbeekMunicipality.Integration.Tests.Persistence;

[Trait("Category", "PostgreSQL")]
public sealed class PostgreSqlMigrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync() => await _postgres.StartAsync();

    public async Task DisposeAsync() => await _postgres.DisposeAsync();

    [Fact]
    public async Task Migrations_ApplySuccessfullyOnPostgreSql()
    {
        var options = new DbContextOptionsBuilder<MunicipalDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .UseSnakeCaseNamingConvention()
            .Options;

        await using var dbContext = new MunicipalDbContext(options);
        await dbContext.Database.MigrateAsync();

        var applied = await dbContext.Database.GetAppliedMigrationsAsync();
        applied.Should().NotBeEmpty();
        applied.Should().Contain(m => m.Contains("CaseIntakeAndIdentity", StringComparison.Ordinal));
    }
}
