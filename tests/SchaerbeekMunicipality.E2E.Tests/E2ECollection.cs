using SchaerbeekMunicipality.E2E.Tests.Infrastructure;

namespace SchaerbeekMunicipality.E2E.Tests;

[CollectionDefinition(Name)]
public sealed class E2ECollection : ICollectionFixture<MunicipalE2EFixture>
{
    public const string Name = "Municipal E2E";
}

[Collection(E2ECollection.Name)]
public abstract class E2ETestBase
{
    protected E2ETestBase(MunicipalE2EFixture fixture)
    {
        Fixture = fixture;
    }

    protected MunicipalE2EFixture Fixture { get; }
}