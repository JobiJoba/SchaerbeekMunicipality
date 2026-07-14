using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Events;

public sealed record RegistrationConfirmed(
    RegistrationCaseId CaseId,
    PersonId PersonId,
    RegisterTarget RegisterTarget,
    DateTimeOffset OccurredAt) : IDomainEvent;