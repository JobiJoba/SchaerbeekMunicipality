using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

internal static class ResidenceDocumentRules
{
    public static bool HasIdentityDocument(IReadOnlyList<DocumentType> documentTypes)
    {
        return documentTypes.Contains(DocumentType.Passport)
               || documentTypes.Contains(DocumentType.NationalIdCard);
    }

    public static ResidencePolicyResult RequireIdentityDocument(IReadOnlyList<DocumentType> documentTypes)
    {
        if (HasIdentityDocument(documentTypes)) return ResidencePolicyResult.Valid();

        return ResidencePolicyResult.Invalid(
            "A passport or national identity card must be attached.");
    }

    public static bool HasDiplomaticIdentityDocument(IReadOnlyList<DocumentType> documentTypes)
    {
        return documentTypes.Contains(DocumentType.Passport)
               || documentTypes.Contains(DocumentType.DiplomaticCard);
    }

    public static ResidencePolicyResult RequireDiplomaticIdentityDocument(
        IReadOnlyList<DocumentType> documentTypes)
    {
        if (HasDiplomaticIdentityDocument(documentTypes)) return ResidencePolicyResult.Valid();

        return ResidencePolicyResult.Invalid(
            "A passport or diplomatic card must be attached.");
    }
}