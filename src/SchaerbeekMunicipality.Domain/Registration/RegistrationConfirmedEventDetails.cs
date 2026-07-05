using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Registration;

public sealed record RegistrationConfirmedEventDetails(
    RegistrationCaseId CaseId,
    PersonId PersonId,
    RegisterTarget RegisterTarget,
    DateTimeOffset ConfirmedAt);
