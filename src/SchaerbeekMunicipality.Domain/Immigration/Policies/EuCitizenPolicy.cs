namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class EuCitizenPolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.EuCitizen;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        var identityResult = ResidenceDocumentRules.RequireIdentityDocument(context.AttachedDocumentTypes);
        if (!identityResult.IsValid) return identityResult;

        return ResidencePolicyResult.Valid();
    }
}