using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.Components;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

public sealed class RegistrationIntakeStepSummariesTests
{
    [Fact]
    public void GetDefaultExpandedSteps_FreshCase_ExpandsIdentityOnly()
    {
        var dto = CreateCase(checklist: EmptyChecklist(identityEstablished: false));

        var expanded = RegistrationIntakeStepSummaries.GetDefaultExpandedSteps(dto);

        expanded.Should().BeEquivalentTo([RegistrationIntakeStep.Identity]);
    }

    [Fact]
    public void GetDefaultExpandedSteps_IdentityComplete_ExpandsLegalResidence()
    {
        var dto = CreateCase(
            checklist: EmptyChecklist(identityEstablished: true),
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
            checklist: EmptyChecklist(
                identityEstablished: true,
                legalResidenceEstablished: true,
                addressDeclared: true),
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
            checklist: EmptyChecklist(
                identityEstablished: true,
                legalResidenceEstablished: true,
                addressDeclared: true),
            person: SamplePerson(),
            residenceCategory: ResidenceCategory.EuCitizen,
            declaredAddress: SampleAddress(),
            housingSituation: HousingSituation.Owner,
            householdMembers: [SampleHouseholdMember()],
            civilStatus: SampleCivilStatus(),
            birthInformation: new BirthInformationDto("Brussels", "Belgium"),
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

    private static RegistrationCaseChecklistDto EmptyChecklist(
        bool identityEstablished = false,
        bool legalResidenceEstablished = false,
        bool addressDeclared = false,
        bool addressConfirmed = false,
        bool registerDeterminable = false,
        bool birthEvidenceEstablished = false,
        bool duplicateInvestigationResolved = true) =>
        new(
            identityEstablished,
            legalResidenceEstablished,
            addressDeclared,
            addressConfirmed,
            registerDeterminable,
            birthEvidenceEstablished,
            duplicateInvestigationResolved);

    private static RegistrationCaseChecklistDto CompleteChecklist() =>
        EmptyChecklist(
            identityEstablished: true,
            legalResidenceEstablished: true,
            addressDeclared: true,
            birthEvidenceEstablished: true);

    private static PersonDto SamplePerson(BirthInformationDto? birthInformation = null) =>
        new(
            Guid.NewGuid(),
            "Marie",
            "Leclerc",
            new DateOnly(1975, 1, 1),
            "Belgian",
            null,
            null,
            false,
            birthInformation);

    private static CivilStatusDto SampleCivilStatus() =>
        new(
            CivilStatus.Single,
            CivilStatus.Single,
            null,
            null,
            null,
            null,
            MarriageRecognitionStatus.NotApplicable);

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
        BirthInformationDto? birthInformation = null,
        PoliceVerificationDto? activePoliceVerification = null) =>
        new(
            Id: Guid.NewGuid(),
            Status: status,
            VisitReason: VisitReason.FirstRegistration,
            AssignedOfficerId: Guid.NewGuid(),
            LockedByOfficerId: Guid.NewGuid(),
            LockedAt: DateTimeOffset.UtcNow,
            CanEdit: true,
            IsReadOnlyDueToLock: false,
            OpenedAt: DateTimeOffset.UtcNow,
            ClosedAt: null,
            Checklist: checklist ?? EmptyChecklist(),
            IsReadyForApproval: false,
            IllegalStayDetected: false,
            MarriageRecognitionBlocking: false,
            DuplicateInvestigationStatus: DuplicateInvestigationStatus.None,
            SuggestedRegisterTarget: null,
            SelectedRegisterTarget: null,
            RejectionReason: null,
            SuspensionReason: null,
            DecisionNotes: null,
            Person: person is null
                ? null
                : birthInformation is null
                    ? person
                    : person with { BirthInformation = birthInformation },
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
