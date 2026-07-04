namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class NonEuWorkerPolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.NonEuWorker;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        if (context.Permit is null)
        {
            return ResidencePolicyResult.Invalid(
                "A valid residence permit is required for non-EU workers.");
        }

        if (context.Permit.PermitType is not (ResidencePermitType.ACard or ResidencePermitType.BCard))
        {
            return ResidencePolicyResult.Invalid(
                "Non-EU workers must hold an A or B card.");
        }

        if (context.Permit.ValidUntil < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return ResidencePolicyResult.Invalid("The residence permit has expired.");
        }

        return ResidencePolicyResult.Valid();
    }
}
