using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Domain.Registration;

public static class RegistrationExceptionRules
{
    public const int DuplicateInvestigationThreshold = NationalRegisterSearchScorer.ExactMatchScore - 20;

    public static bool HasBirthCertificate(IReadOnlyList<DocumentType> documentTypes) =>
        documentTypes.Contains(DocumentType.BirthCertificate);

    public static bool IsBirthEvidenceComplete(Person? person, IReadOnlyList<DocumentType> documentTypes) =>
        person?.BirthPlace is not null
        && HasBirthCertificate(documentTypes);

    public static bool RequiresDuplicateInvestigation(
        Person? person,
        IReadOnlyList<NationalRegisterMatch> matches) =>
        person is not null
        && person.LinkedRegisterRecordId is null
        && matches.Any(m => m.MatchScore >= DuplicateInvestigationThreshold);

    public static bool IsMarriageAbroad(CivilStatusRecord? civilStatus) =>
        civilStatus is not null
        && CivilStatusRecord.RequiresMarriageDetails(civilStatus.Status)
        && !string.IsNullOrWhiteSpace(civilStatus.MarriagePlace)
        && !IsBelgianMarriagePlace(civilStatus.MarriagePlace);

    public static bool IsMarriageRecognitionBlocking(CivilStatusRecord? civilStatus) =>
        civilStatus is not null
        && IsMarriageAbroad(civilStatus)
        && civilStatus.MarriageRecognitionStatus == MarriageRecognitionStatus.PendingRecognition;

    public static bool IsIllegalStay(
        RegistrationCase registrationCase,
        ResidencePolicyResult? policyResult) =>
        registrationCase.ResidenceCategory is not null
        && registrationCase.ResidenceCategory != ResidenceCategory.Refugee
        && registrationCase.ImmigrationDecision is null
        && policyResult is { IsValid: false };

    private static bool IsBelgianMarriagePlace(string marriagePlace)
    {
        var normalized = marriagePlace.Trim();
        return normalized.Contains("Belgium", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("België", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains("Belgique", StringComparison.OrdinalIgnoreCase);
    }
}
