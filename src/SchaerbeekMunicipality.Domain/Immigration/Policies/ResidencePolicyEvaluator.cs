using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class ResidencePolicyEvaluator
{
    private readonly IReadOnlyDictionary<ResidenceCategory, IResidencePolicy> _policies;

    public ResidencePolicyEvaluator(IEnumerable<IResidencePolicy> policies)
    {
        _policies = policies.ToDictionary(p => p.Category);
    }

    public ResidencePolicyResult Evaluate(
        RegistrationCase registrationCase,
        ResidencePermit? permit,
        IReadOnlyList<DocumentType> attachedDocumentTypes)
    {
        if (registrationCase.ResidenceCategory is not { } category)
        {
            return ResidencePolicyResult.Invalid("Residence category has not been set.");
        }

        if (!_policies.TryGetValue(category, out var policy))
        {
            return ResidencePolicyResult.Invalid($"No policy is configured for category '{category}'.");
        }

        var context = new ResidenceValidationContext(
            category,
            permit,
            registrationCase.ImmigrationDecision,
            attachedDocumentTypes);

        return policy.Validate(context);
    }
}
