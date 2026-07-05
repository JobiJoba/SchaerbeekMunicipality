using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Web.Features.Registration.LinkExistingPerson;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;
using SchaerbeekMunicipality.Web.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class SearchNationalRegisterTests
{
    [Fact]
    public async Task SearchNationalRegister_PartialGivenName_ReturnsMatches()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<SearchNationalRegisterHandler>();

        var response = await handler.Handle(
            new SearchNationalRegisterRequest("Marie", null, null),
            CancellationToken.None);

        response.Matches.Should().ContainSingle();
        response.Matches[0].GivenName.Should().Be("Marie");
    }

    [Fact]
    public async Task SearchNationalRegister_ReturnsScoredMatches()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<SearchNationalRegisterHandler>();

        var response = await handler.Handle(
            new SearchNationalRegisterRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20)),
            CancellationToken.None);

        response.Matches.Should().NotBeEmpty();
        response.Matches[0].MatchScore.Should().BeGreaterOrEqualTo(80);
        response.Matches[0].BisNumber.Should().NotBeNullOrWhiteSpace();
    }
}

public sealed class LinkExistingPersonTests
{
    [Fact]
    public async Task LinkExistingPerson_LinksBisRecordToCase()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var linkHandler = scope.ServiceProvider.GetRequiredService<LinkExistingPersonHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        var response = await linkHandler.Handle(
            caseId,
            new LinkExistingPersonRequest(NationalRegisterSeeder.MarieLeclercId.Value),
            CancellationToken.None);

        response.IdentityEstablished.Should().BeTrue();
        response.BisNumber.Should().NotBeNullOrWhiteSpace();
        response.LinkedFromRegister.Should().BeTrue();
    }

    [Fact]
    public async Task LinkExistingPerson_WhenNrAlreadyAssigned_ThrowsConflict()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var linkHandler = scope.ServiceProvider.GetRequiredService<LinkExistingPersonHandler>();

        var firstCaseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        await linkHandler.Handle(
            firstCaseId,
            new LinkExistingPersonRequest(NationalRegisterSeeder.JeanDupontId.Value),
            CancellationToken.None);

        var secondCaseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        var act = () => linkHandler.Handle(
            secondCaseId,
            new LinkExistingPersonRequest(NationalRegisterSeeder.JeanDupontId.Value),
            CancellationToken.None);

        await act.Should().ThrowAsync<NationalRegisterConflictException>();
    }

    [Fact]
    public async Task RecordIdentity_ThenSearch_FindsPossibleDuplicate()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var openHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var recordHandler = scope.ServiceProvider.GetRequiredService<RecordIdentityHandler>();
        var searchHandler = scope.ServiceProvider.GetRequiredService<SearchNationalRegisterHandler>();

        var caseId = await RegistrationTestHelpers.OpenAndClaimCaseAsync(scope.ServiceProvider);

        await recordHandler.Handle(
            caseId,
            new RecordIdentityRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20), "French"),
            CancellationToken.None);

        var search = await searchHandler.Handle(
            new SearchNationalRegisterRequest("Amélie", "Bernard", new DateOnly(1992, 3, 20)),
            CancellationToken.None);

        search.Matches.Should().Contain(m => m.MatchScore >= 100);
    }
}
