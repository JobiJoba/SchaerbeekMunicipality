namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class DiplomatPolicy : IResidencePolicy
{
    public ResidenceCategory Category => ResidenceCategory.Diplomat;

    public ResidencePolicyResult Validate(ResidenceValidationContext context)
    {
        return ResidenceDocumentRules.RequireDiplomaticIdentityDocument(context.AttachedDocumentTypes);
    }
}
