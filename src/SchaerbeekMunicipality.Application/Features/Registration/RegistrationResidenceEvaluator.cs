using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration;

public sealed class RegistrationResidenceEvaluator(
    ResidencePolicyEvaluator policyEvaluator,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository)
{
    public async Task<IReadOnlyList<DocumentType>> GetAttachedDocumentTypesAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var documents = await documentRepository.ListByRegistrationCaseIdAsync(caseId, cancellationToken);
        return documents.Select(d => d.DocumentType).ToList();
    }

    public async Task<ResidencePolicyResult> EvaluateAndApplyAsync(
        RegistrationCase registrationCase,
        CancellationToken cancellationToken,
        ResidencePermit? permitOverride = null,
        IReadOnlyList<DocumentType>? documentTypesOverride = null)
    {
        var permit = permitOverride
            ?? await permitRepository.GetByCaseIdAsync(registrationCase.Id, cancellationToken);
        var documentTypes = documentTypesOverride
            ?? await GetAttachedDocumentTypesAsync(registrationCase.Id, cancellationToken);

        var result = policyEvaluator.Evaluate(registrationCase, permit, documentTypes);
        registrationCase.ApplyResidencePolicyResult(result);

        return result;
    }
}
