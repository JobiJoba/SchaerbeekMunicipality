namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class EuCitizenPolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.EuCitizen;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        return ResidencePolicyResult.Valid();
    }
}
