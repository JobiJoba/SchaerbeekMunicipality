using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.ChangeOfAddress;

public sealed class ChangeOfAddressCaseTests
{
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 9, 10, 0, 0, TimeSpan.Zero);
    private static readonly PersonId PersonId = PersonId.New();

    [Fact]
    public void Open_SetsPersonIdentifiedAndIntakeStatus()
    {
        var changeOfAddressCase = ChangeOfAddressCase.Open(PersonId, null, OpenedAt);

        changeOfAddressCase.Status.Should().Be(ChangeOfAddressCaseStatus.Intake);
        changeOfAddressCase.PersonId.Should().Be(PersonId);
        changeOfAddressCase.Checklist.PersonIdentified.Should().BeTrue();
    }

    [Fact]
    public void DeclareNewAddress_InvalidPostalCode_Throws()
    {
        var changeOfAddressCase = ChangeOfAddressCase.Open(PersonId, null, OpenedAt);
        changeOfAddressCase.Claim(OfficerId.From(Guid.NewGuid()), OpenedAt);

        var act = () => changeOfAddressCase.DeclareNewAddress(
            BelgianAddress.Create("Rue Example", "1", null, "1000", "Brussels"),
            HousingSituation.Owner,
            new DateOnly(2026, 7, 15));

        act.Should().Throw<InvalidChangeOfAddressTransitionException>();
    }

    [Fact]
    public void ConfirmAddressChange_WhilePolicePending_Throws()
    {
        var changeOfAddressCase = CreateCaseReadyExceptPolice();
        changeOfAddressCase.RequestPoliceVerification();

        var act = () => changeOfAddressCase.ConfirmAddressChange(OpenedAt.AddHours(2));

        act.Should().Throw<InvalidChangeOfAddressTransitionException>();
    }

    [Fact]
    public void HappyPath_OpenDeclareConfirm()
    {
        var changeOfAddressCase = ChangeOfAddressCase.Open(PersonId, null, OpenedAt);
        changeOfAddressCase.Claim(OfficerId.From(Guid.NewGuid()), OpenedAt);

        changeOfAddressCase.DeclareNewAddress(
            BelgianAddress.Create("Avenue Rogier", "10", null, "1030", "Schaerbeek"),
            HousingSituation.Owner,
            new DateOnly(2026, 7, 15));

        changeOfAddressCase.Status.Should().Be(ChangeOfAddressCaseStatus.UnderReview);
        changeOfAddressCase.IsReadyForConfirmation.Should().BeTrue();

        var details = changeOfAddressCase.ConfirmAddressChange(OpenedAt.AddHours(1));

        changeOfAddressCase.Status.Should().Be(ChangeOfAddressCaseStatus.Confirmed);
        details.PersonId.Should().Be(PersonId);
    }

    [Fact]
    public void ApplyPoliceVerificationResult_Confirmed_MovesToUnderReview()
    {
        var changeOfAddressCase = CreateCaseWithAddress();
        changeOfAddressCase.RequestPoliceVerification();

        changeOfAddressCase.ApplyPoliceVerificationResult(PoliceVerificationResult.Confirmed);

        changeOfAddressCase.Status.Should().Be(ChangeOfAddressCaseStatus.UnderReview);
        changeOfAddressCase.Checklist.PoliceVerificationPositive.Should().BeTrue();
    }

    private static ChangeOfAddressCase CreateCaseWithAddress()
    {
        var changeOfAddressCase = ChangeOfAddressCase.Open(PersonId, null, OpenedAt);
        changeOfAddressCase.Claim(OfficerId.From(Guid.NewGuid()), OpenedAt);
        changeOfAddressCase.DeclareNewAddress(
            BelgianAddress.Create("Avenue Rogier", "10", null, "1030", "Schaerbeek"),
            HousingSituation.Owner,
            new DateOnly(2026, 7, 15));
        return changeOfAddressCase;
    }

    private static ChangeOfAddressCase CreateCaseReadyExceptPolice() => CreateCaseWithAddress();
}
