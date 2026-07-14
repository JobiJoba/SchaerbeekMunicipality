namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class RefugeePolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.Refugee;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        if (context.ImmigrationDecision is null)
            return ResidencePolicyResult.Invalid(
                "Awaiting federal protection decision before registration can proceed.");

        return ResidencePolicyResult.Valid();
    }
}