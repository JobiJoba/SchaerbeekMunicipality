using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public sealed record PersonRadiatedEventDetails(
    DeathDeclarationCaseId CaseId,
    PersonId PersonId,
    DateOnly DeathDate,
    DateTimeOffset OccurredAt);
