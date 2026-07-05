using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.Components;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class RegistrationIntakeStepSummariesTests
{
    [Fact]
    public void GetDefaultExpandedSteps_FreshCase_ExpandsIdentityOnly()
    {
        var dto = CreateCase(checklist: new RegistrationCaseChecklistDto(
            IdentityEstablished: false,
            LegalResidenceEstablished: false,
            AddressDeclared: false,
            AddressConfirmed: false,
            RegisterDeterminable: false));

        var expanded = RegistrationIntakeStepSummaries.GetDefaultExpandedSteps(dto);

        expanded.Should().BeEquivalentTo([RegistrationIntakeStep.Identity]);
    }

    [Fact]
    public void GetDefaultExpandedSteps_IdentityComplete_ExpandsLegalResidence()
    {
        var dto = CreateCase(
            checklist: new RegistrationCaseChecklistDto(
                IdentityEstablished: true,
                LegalResidenceEstablished: false,
                AddressDeclared: false,
                AddressConfirmed: false,
                RegisterDeterminable: false),
            person: SamplePerson());

        var expanded = RegistrationIntakeStepSummaries.GetDefaultExpandedSteps(dto);

        expanded.Should().BeEquivalentTo(
        [
            RegistrationIntakeStep.Identity,
            RegistrationIntakeStep.LegalResidence,
        ]);
    }

    [Fact]
    public void GetDefaultExpandedSteps_AddressCompleteNoHousehold_ExpandsHousehold()
    {
        var dto = CreateCase(
            checklist: new RegistrationCaseChecklistDto(
                IdentityEstablished: true,
                LegalResidenceEstablished: true,
                AddressDeclared: true,
                AddressConfirmed: false,
                RegisterDeterminable: false),
            person: SamplePerson(),
            residenceCategory: ResidenceCategory.EuCitizen,
            declaredAddress: SampleAddress(),
            housingSituation: HousingSituation.Owner);

        var expanded = RegistrationIntakeStepSummaries.GetDefaultExpandedSteps(dto);

        expanded.Should().BeEquivalentTo(
        [
            RegistrationIntakeStep.Identity,
            RegistrationIntakeStep.Household,
        ]);
    }

    [Fact]
    public void GetDefaultExpandedSteps_AwaitingPolice_ExpandsPoliceVerification()
    {
        var dto = CreateCase(
            status: RegistrationCaseStatus.AwaitingPoliceVerification,
            checklist: new RegistrationCaseChecklistDto(
                IdentityEstablished: true,
                LegalResidenceEstablished: true,
                AddressDeclared: true,
                AddressConfirmed: false,
                RegisterDeterminable: false),
            person: SamplePerson(),
            residenceCategory: ResidenceCategory.EuCitizen,
            declaredAddress: SampleAddress(),
            housingSituation: HousingSituation.Owner,
            householdMembers: [SampleHouseholdMember()],
            civilStatus: new CivilStatusDto(CivilStatus.Single, null, null, null, null),
            activePoliceVerification: new PoliceVerificationDto(
                Guid.NewGuid(),
                1,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                true));

        var expanded = RegistrationIntakeStepSummaries.GetDefaultExpandedSteps(dto);

        expanded.Should().BeEquivalentTo(
        [
            RegistrationIntakeStep.Identity,
            RegistrationIntakeStep.PoliceVerification,
        ]);
    }

    [Theory]
    [InlineData(false, false, "Not recorded")]
    [InlineData(true, true, "Marie Leclerc")]
    public void GetSummary_Identity_ReflectsRecordedState(bool hasPerson, bool _, string expectedFragment)
    {
        var dto = CreateCase(
            checklist: CompleteChecklist(),
            person: hasPerson ? SamplePerson() : null);

        var summary = RegistrationIntakeStepSummaries.GetSummary(RegistrationIntakeStep.Identity, dto);

        summary.Should().Contain(expectedFragment);
    }

    [Fact]
    public void IsVisible_PoliceVerification_HiddenUntilVisitExists()
    {
        var withoutPolice = CreateCase(checklist: CompleteChecklist(), person: SamplePerson());
        var withPolice = withoutPolice with
        {
            ActivePoliceVerification = new PoliceVerificationDto(
                Guid.NewGuid(),
                1,
                DateTimeOffset.UtcNow,
                null,
                null,
                null,
                true),
        };

        RegistrationIntakeStepSummaries.IsVisible(RegistrationIntakeStep.PoliceVerification, withoutPolice)
            .Should().BeFalse();
        RegistrationIntakeStepSummaries.IsVisible(RegistrationIntakeStep.PoliceVerification, withPolice)
            .Should().BeTrue();
    }

    private static RegistrationCaseChecklistDto CompleteChecklist() =>
        new(
            IdentityEstablished: true,
            LegalResidenceEstablished: true,
            AddressDeclared: true,
            AddressConfirmed: false,
            RegisterDeterminable: false);

    private static PersonDto SamplePerson() =>
        new(
            Guid.NewGuid(),
            "Marie",
            "Leclerc",
            new DateOnly(1975, 1, 1),
            "Belgian",
            null,
            null,
            false);

    private static BelgianAddressDto SampleAddress() =>
        new("Chaussée de Louvain", "42", null, "1030", "Schaerbeek");

    private static HouseholdMemberDto SampleHouseholdMember() =>
        new(Guid.NewGuid(), "Jean", "Dupont", new DateOnly(2010, 5, 5), HouseholdMemberRole.Child);

    private static RegistrationCaseDetailDto CreateCase(
        RegistrationCaseStatus status = RegistrationCaseStatus.Intake,
        RegistrationCaseChecklistDto? checklist = null,
        PersonDto? person = null,
        ResidenceCategory? residenceCategory = null,
        BelgianAddressDto? declaredAddress = null,
        HousingSituation? housingSituation = null,
        IReadOnlyList<HouseholdMemberDto>? householdMembers = null,
        CivilStatusDto? civilStatus = null,
        PoliceVerificationDto? activePoliceVerification = null) =>
        new(
            Id: Guid.NewGuid(),
            Status: status,
            VisitReason: VisitReason.FirstRegistration,
            AssignedOfficerId: Guid.NewGuid(),
            OpenedAt: DateTimeOffset.UtcNow,
            ClosedAt: null,
            Checklist: checklist ?? new RegistrationCaseChecklistDto(false, false, false, false, false),
            IsReadyForApproval: false,
            SuggestedRegisterTarget: null,
            SelectedRegisterTarget: null,
            RejectionReason: null,
            SuspensionReason: null,
            DecisionNotes: null,
            Person: person,
            ResidenceCategory: residenceCategory,
            ResidencePermit: null,
            ImmigrationDecision: null,
            DeclaredAddress: declaredAddress,
            HousingSituation: housingSituation,
            HouseholdMembers: householdMembers ?? [],
            CivilStatus: civilStatus,
            Documents: [],
            PossibleDuplicateMatches: [],
            ActivePoliceVerification: activePoliceVerification,
            PoliceVerificationHistory: []);
}
