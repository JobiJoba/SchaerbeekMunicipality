using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration;

public sealed class RegistrationResidenceEvaluator(
    ResidencePolicyEvaluator policyEvaluator,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository)
{
    public async Task<ResidencePolicyResult> EvaluateAndApplyAsync(
        RegistrationCase registrationCase,
        CancellationToken cancellationToken,
        ResidencePermit? permit = null)
    {
        permit ??= await permitRepository.GetByCaseIdAsync(registrationCase.Id, cancellationToken);
        var documents = await documentRepository.ListByCaseIdAsync(registrationCase.Id, cancellationToken);
        var documentTypes = documents.Select(d => d.DocumentType).ToList();

        var result = policyEvaluator.Evaluate(registrationCase, permit, documentTypes);
        registrationCase.ApplyResidencePolicyResult(result);

        return result;
    }
}
