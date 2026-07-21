using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.ReferenceData;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Tests.Registration;

public sealed class RegistrationCaseDecisionTests
{
    private static readonly OfficerId DemoOfficer = OfficerId.From(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly DateTimeOffset OpenedAt = new(2026, 7, 5, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset DecisionAt = OpenedAt.AddDays(3);

    private static RegistrationCase CreateReadyForApprovalCase(string nationality = "Belgian")
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        var person = registrationCase.RecordIdentity(
            new IdentityDetails("Sophie", "Lambert", new DateOnly(1988, 6, 12), nationality));

        person.RecordBirthInformation(new BirthInformationDetails("Brussels", "Belgium"));
        registrationCase.ApplyBirthEvidenceRule(
            person,
            [DocumentType.BirthCertificate, DocumentType.Passport]);

        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);
        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());
        registrationCase.RefreshRegisterDeterminability(nationality);

        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        registrationCase.RequestPoliceVerification();
        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.Confirmed);

        return registrationCase;
    }

    [Fact]
    public void Approve_WhenReady_TransitionsToApproved()
    {
        var registrationCase = CreateReadyForApprovalCase();

        registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.PopulationRegister,
            "Belgian",
            DecisionAt);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Approved);
        registrationCase.SelectedRegisterTarget.Should().Be(RegisterTarget.PopulationRegister);
    }

    [Fact]
    public void Approve_WhenPoliceResultNegative_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.RecordIdentity(
            new IdentityDetails("Sophie", "Lambert", new DateOnly(1988, 6, 12), "Belgian"));

        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);
        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());
        registrationCase.RefreshRegisterDeterminability("Belgian");

        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        registrationCase.RequestPoliceVerification();
        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.NotFound);

        var act = () => registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.PopulationRegister,
            "Belgian",
            DecisionAt);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void Approve_WithWrongRegisterTarget_Throws()
    {
        var registrationCase = CreateReadyForApprovalCase("French");

        var act = () => registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.PopulationRegister,
            "French",
            DecisionAt);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void Reject_FromIntake_TransitionsToRejected()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();

        registrationCase.Reject(
            DemoOfficer,
            RejectionReason.OpenedInError,
            "Wrong person",
            DecisionAt);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Rejected);
        registrationCase.RejectionReason.Should().Be(RejectionReason.OpenedInError);
    }

    [Fact]
    public void Reject_FromUnderReview_TransitionsToRejected()
    {
        var registrationCase = CreateReadyForApprovalCase();

        registrationCase.Reject(
            DemoOfficer,
            RejectionReason.NoLegalResidenceBasis,
            "Permit expired",
            DecisionAt);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Rejected);
        registrationCase.RejectionReason.Should().Be(RejectionReason.NoLegalResidenceBasis);
        registrationCase.ClosedAt.Should().Be(DecisionAt);
    }

    [Fact]
    public void Suspend_FromUnderReview_CanResume()
    {
        var registrationCase = CreateReadyForApprovalCase();

        registrationCase.Suspend(
            DemoOfficer,
            SuspensionReason.MissingDocuments,
            "Birth certificate",
            DecisionAt);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Suspended);

        registrationCase.ResumeFromSuspension();

        registrationCase.Status.Should().Be(RegistrationCaseStatus.UnderReview);
        registrationCase.SuspensionReason.Should().BeNull();
    }

    [Fact]
    public void ConfirmRegistration_FromApproved_TransitionsToRegistered()
    {
        var registrationCase = CreateReadyForApprovalCase();
        registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.PopulationRegister,
            "Belgian",
            DecisionAt);

        var details = registrationCase.ConfirmRegistration(DecisionAt.AddHours(1));

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Registered);
        details.RegisterTarget.Should().Be(RegisterTarget.PopulationRegister);
        registrationCase.ClosedAt.Should().NotBeNull();
    }

    [Fact]
    public void Approve_WhenDuplicateInvestigationOpen_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        var person = registrationCase.RecordIdentity(
            new IdentityDetails("Jean", "Dupont", new DateOnly(1985, 6, 12), "Belgian"));
        person.RecordBirthInformation(new BirthInformationDetails("Brussels", "Belgium"));
        registrationCase.ApplyBirthEvidenceRule(
            person,
            [DocumentType.BirthCertificate, DocumentType.Passport]);
        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);
        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());
        registrationCase.RefreshRegisterDeterminability("Belgian");
        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));
        registrationCase.RequestPoliceVerification();
        registrationCase.ApplyPoliceVerificationResult(PoliceVerificationResult.Confirmed);

        registrationCase.ApplyDuplicateInvestigationRule(
            person,
            [
                new NationalRegisterMatch(
                    NationalRegisterPersonId.New(),
                    "Jean",
                    "Dupont",
                    new DateOnly(1985, 6, 12),
                    "Belgian",
                    null,
                    null,
                    NationalRegisterSearchScorer.ExactMatchScore,
                    "Exact match")
            ]);

        registrationCase.DuplicateInvestigationStatus.Should().Be(DuplicateInvestigationStatus.Open);

        var act = () => registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.PopulationRegister,
            "Belgian",
            DecisionAt);

        act.Should().Throw<InvalidRegistrationTransitionException>()
            .WithMessage("*duplicate*");
    }

    [Fact]
    public void ConfirmRegistration_WithoutApproval_Throws()
    {
        var registrationCase = CreateReadyForApprovalCase();

        var act = () => registrationCase.ConfirmRegistration(DecisionAt);

        act.Should().Throw<InvalidRegistrationTransitionException>();
    }

    [Fact]
    public void WaivePoliceVerification_ForDiplomat_ConfirmsAddress()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Elena", "Morales", new DateOnly(1979, 3, 8), "Spanish"));
        registrationCase.SetResidenceCategory(ResidenceCategory.Diplomat);
        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());
        registrationCase.RefreshRegisterDeterminability("Spanish");
        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        registrationCase.WaivePoliceVerification();

        registrationCase.Checklist.AddressConfirmed.Should().BeTrue();
        registrationCase.Status.Should().Be(RegistrationCaseStatus.UnderReview);
    }

    [Fact]
    public void RequestPoliceVerification_ForDiplomat_Throws()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Elena", "Morales", new DateOnly(1979, 3, 8), "Spanish"));
        registrationCase.SetResidenceCategory(ResidenceCategory.Diplomat);
        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        var act = () => registrationCase.RequestPoliceVerification();

        act.Should().Throw<InvalidRegistrationTransitionException>()
            .WithMessage("*waived*");
    }

    [Fact]
    public void SetResidenceCategory_LeavingDiplomat_ClearsAddressConfirmed()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Elena", "Morales", new DateOnly(1979, 3, 8), "Spanish"));
        registrationCase.SetResidenceCategory(ResidenceCategory.Diplomat);
        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));
        registrationCase.WaivePoliceVerification();

        registrationCase.SetResidenceCategory(ResidenceCategory.EuCitizen);

        registrationCase.Checklist.AddressConfirmed.Should().BeFalse();
    }

    [Fact]
    public void DeclareReferenceAddress_MarksAddressDeclaredAsReference()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Marc", "Dupont", new DateOnly(1975, 9, 12), "Belgian"));

        registrationCase.DeclareReferenceAddress();

        registrationCase.Checklist.AddressDeclared.Should().BeTrue();
        registrationCase.AddressDeclarationType.Should().Be(AddressDeclarationType.ReferenceAddress);
        registrationCase.DeclaredAddress!.Street.Should().Be(SchaerbeekCommune.ReferenceStreet);
        registrationCase.DeclaredAddress.HouseNumber.Should().Be(SchaerbeekCommune.ReferenceHouseNumber);
    }

    [Fact]
    public void DeclareAddress_AfterReferenceAddress_SwitchesToDomicile()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        registrationCase.RecordIdentity(
            new IdentityDetails("Marc", "Dupont", new DateOnly(1975, 9, 12), "Belgian"));
        registrationCase.DeclareReferenceAddress();

        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));

        registrationCase.AddressDeclarationType.Should().Be(AddressDeclarationType.Domicile);
        registrationCase.DeclaredAddress!.Street.Should().Be("Chaussée de Louvain");
        registrationCase.Checklist.AddressConfirmed.Should().BeFalse();
    }

    [Fact]
    public void Approve_Diplomat_WithSpecialRegister_Succeeds()
    {
        var registrationCase = RegistrationCaseTestData.OpenClaimedCase();
        var person = registrationCase.RecordIdentity(
            new IdentityDetails("Elena", "Morales", new DateOnly(1979, 3, 8), "Spanish"));
        person.RecordBirthInformation(new BirthInformationDetails("Madrid", "Spain"));
        registrationCase.ApplyBirthEvidenceRule(
            person,
            [DocumentType.BirthCertificate, DocumentType.DiplomaticCard]);
        registrationCase.SetResidenceCategory(ResidenceCategory.Diplomat);
        registrationCase.ApplyResidencePolicyResult(ResidencePolicyResult.Valid());
        registrationCase.RefreshRegisterDeterminability("Spanish");
        registrationCase.DeclareAddress(new AddressDetails(
            "Chaussée de Louvain",
            "10",
            null,
            "1030",
            "Schaerbeek"));
        registrationCase.WaivePoliceVerification();

        registrationCase.Approve(
            DemoOfficer,
            RegisterTarget.SpecialRegister,
            "Spanish",
            DecisionAt);

        registrationCase.Status.Should().Be(RegistrationCaseStatus.Approved);
        registrationCase.SelectedRegisterTarget.Should().Be(RegisterTarget.SpecialRegister);
    }
}