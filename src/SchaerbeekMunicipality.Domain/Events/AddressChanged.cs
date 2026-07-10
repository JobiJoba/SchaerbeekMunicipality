using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Events;

public sealed record AddressChanged(
    ChangeOfAddressCaseId CaseId,
    PersonId PersonId,
    DateTimeOffset OccurredAt) : IDomainEvent;
