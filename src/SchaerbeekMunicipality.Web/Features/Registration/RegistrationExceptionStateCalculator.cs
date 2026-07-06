using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public static class RegistrationExceptionStateCalculator
{
    public static RegistrationExceptionDisplayState Calculate(
        RegistrationCase registrationCase,
        Person? person,
        IReadOnlyList<DocumentType> documentTypes,
        ResidencePolicyResult policyResult)
    {
        var birthEvidenceEstablished = RegistrationExceptionRules.IsBirthEvidenceComplete(
            person,
            documentTypes);

        var marriageRecognitionBlocking = registrationCase.IsMarriageRecognitionBlocking(person);
        var illegalStayDetected = registrationCase.IsIllegalStayDetected(policyResult);

        var isReadyForApproval =
            registrationCase.Status == RegistrationCaseStatus.UnderReview
            && registrationCase.Checklist.IdentityEstablished
            && registrationCase.Checklist.LegalResidenceEstablished
            && registrationCase.Checklist.AddressDeclared
            && registrationCase.Checklist.AddressConfirmed
            && registrationCase.Checklist.RegisterDeterminable
            && birthEvidenceEstablished
            && registrationCase.Checklist.DuplicateInvestigationResolved
            && registrationCase.DuplicateInvestigationStatus != DuplicateInvestigationStatus.Open
            && registrationCase.HasPositivePoliceVerification
            && !marriageRecognitionBlocking
            && !illegalStayDetected;

        return new RegistrationExceptionDisplayState(
            birthEvidenceEstablished,
            illegalStayDetected,
            marriageRecognitionBlocking,
            isReadyForApproval,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}

public sealed record RegistrationExceptionDisplayState(
    bool BirthEvidenceEstablished,
    bool IllegalStayDetected,
    bool MarriageRecognitionBlocking,
    bool IsReadyForApproval,
    string? ResidencePolicyFailureReason);
