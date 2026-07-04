namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public interface IResidencePolicy
{
    ResidenceCategory Category { get; }

    ResidencePolicyResult Validate(ResidenceValidationContext context);
}
