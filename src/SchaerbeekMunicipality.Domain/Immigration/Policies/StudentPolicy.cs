namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class StudentPolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.Student;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        var identityResult = ResidenceDocumentRules.RequireIdentityDocument(context.AttachedDocumentTypes);
        if (!identityResult.IsValid)
        {
            return identityResult;
        }

        if (context.Permit is null)
        {
            return ResidencePolicyResult.Invalid(
                "Students must provide a residence permit or Annex 15.");
        }

        if (context.Permit.PermitType is not (ResidencePermitType.Annex15 or ResidencePermitType.BCard))
        {
            return ResidencePolicyResult.Invalid(
                "Students must hold Annex 15 or a B card.");
        }

        if (context.Permit.ValidUntil < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return ResidencePolicyResult.Invalid("The residence permit has expired.");
        }

        return ResidencePolicyResult.Valid();
    }
}
