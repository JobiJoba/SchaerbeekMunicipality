using FluentAssertions;
using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Police;
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
}