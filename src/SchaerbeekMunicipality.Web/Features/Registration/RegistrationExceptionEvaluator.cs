using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public sealed class RegistrationExceptionEvaluator(
    RegistrationResidenceEvaluator residenceEvaluator,
    IPersonRepository personRepository,
    INationalRegisterRepository nationalRegisterRepository)
{
    public async Task<RegistrationExceptionEvaluationResult> EvaluateAndApplyAsync(
        RegistrationCase registrationCase,
        CancellationToken cancellationToken,
        Person? personOverride = null,
        ResidencePermit? permitOverride = null,
        IReadOnlyList<DocumentType>? documentTypesOverride = null)
    {
        Person? person = personOverride;
        if (person is null && registrationCase.PersonId is { } personId)
        {
            person = await personRepository.GetForUpdateAsync(personId, cancellationToken);
        }

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken,
            permitOverride,
            documentTypesOverride);

        var documentTypes = documentTypesOverride
            ?? await residenceEvaluator.GetAttachedDocumentTypesAsync(
                registrationCase.Id,
                cancellationToken);

        registrationCase.ApplyBirthEvidenceRule(person, documentTypes);

        var duplicateMatches = await FindDuplicateMatchesAsync(person, cancellationToken);
        registrationCase.ApplyDuplicateInvestigationRule(person, duplicateMatches);

        if (person is not null)
        {
            registrationCase.RefreshRegisterDeterminability(person.Nationality);
        }

        var illegalStayDetected = registrationCase.IsIllegalStayDetected(policyResult);
        var marriageRecognitionBlocking = registrationCase.IsMarriageRecognitionBlocking(person);

        return new RegistrationExceptionEvaluationResult(
            policyResult,
            illegalStayDetected,
            marriageRecognitionBlocking);
    }

    private async Task<IReadOnlyList<NationalRegisterMatch>> FindDuplicateMatchesAsync(
        Person? person,
        CancellationToken cancellationToken)
    {
        if (person is null || person.LinkedRegisterRecordId is not null)
        {
            return [];
        }

        var criteria = new NationalRegisterSearchCriteria(
            person.GivenName,
            person.FamilyName,
            person.BirthDate);

        return await nationalRegisterRepository.SearchAsync(criteria, cancellationToken);
    }
}

public sealed record RegistrationExceptionEvaluationResult(
    ResidencePolicyResult PolicyResult,
    bool IllegalStayDetected,
    bool MarriageRecognitionBlocking);
