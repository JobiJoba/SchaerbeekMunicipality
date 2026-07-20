using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Events;

public sealed record PersonRadiated(
    DeathDeclarationCaseId CaseId,
    PersonId PersonId,
    DateOnly DeathDate,
    DateTimeOffset OccurredAt) : IDomainEvent;
