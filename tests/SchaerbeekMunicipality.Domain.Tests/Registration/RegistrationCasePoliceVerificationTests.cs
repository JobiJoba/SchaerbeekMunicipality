using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCasePoliceVerificationTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 4, 10, 0, 0, TimeSpan.Zero);

    private static RegistrationCase CreateCaseWithIdentityAndAddress()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"));

        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        return registrationCase;
    }

    [Fact]
    public void RequestPoliceVerification_WhenReady_TransitionsToAwaitingPolice()
    {
        var registrationCase = CreateCaseWithIdentityAndAddress();

        registrationCase.RequestPoliceVerification();

        registrationCase.Status.Should().Be(RegistrationCaseStatus.AwaitingPoliceVerification);
    }

    [Fact]
    public void RequestPoliceVerification_WithoutAddress_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"));

        var act = () => registrationCase.RequestPoliceVerification();

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void ApplyPoliceVerificationResult_Confirmed_MarksAddressConfirmed()
    {
        var registrationCase = CreateCaseWithIdentityAndAddress();
        registrationCase.RequestPoliceVerification();

        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.Confirmed);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.UnderReview);
        registrationCase.Checklist.AddressConfirmed.Should().BeTrue();
        registrationCase.HasPositivePoliceVerification.Should().BeTrue();
    }

    [Fact]
    public void ApplyPoliceVerificationResult_NotFound_ClearsAddressConfirmed()
    {
        var registrationCase = CreateCaseWithIdentityAndAddress();
        registrationCase.RequestPoliceVerification();

        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.NotFound);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.UnderReview);
        registrationCase.Checklist.AddressConfirmed.Should().BeFalse();
        registrationCase.HasPositivePoliceVerification.Should().BeFalse();
    }

    [Fact]
    public void RequestPoliceVerification_FromUnderReview_AllowsSecondAttempt()
    {
        var registrationCase = CreateCaseWithIdentityAndAddress();
        registrationCase.RequestPoliceVerification();
        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.Incomplete);

        registrationCase.RequestPoliceVerification();

        registrationCase.Status.Should().Be(RegistrationCaseStatus.AwaitingPoliceVerification);
    }

    [Fact]
    public void PoliceVerificationRequest_RecordResult_WhenAlreadyCompleted_Throws()
    {
        var request = PoliceVerificationRequest.Create(
            RegistrationCaseId.New(),
            1,
            OpenedAt);

        request.RecordResult(PoliceVerificationResult.Confirmed, null, OpenedAt.AddHours(2));

        var act = () => request.RecordResult(PoliceVerificationResult.NotFound, null, OpenedAt.AddHours(3));

        act.Should().Throw<InvalidPoliceVerificationException>();
    }
}