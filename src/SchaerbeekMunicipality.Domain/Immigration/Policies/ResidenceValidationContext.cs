using SchaerbeekMunicipality.Domain.Documents;

namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed record ResidenceValidationContext(
    ResidenceCategory Category,
    ResidencePermit? Permit,
    ImmigrationDecisionReference? ImmigrationDecision,
    IReadOnlyList<DocumentType> AttachedDocumentTypes);
