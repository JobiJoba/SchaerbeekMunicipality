using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.Events;

public sealed record AddressChanged(
    ChangeOfAddressCaseId CaseId,
    PersonId PersonId,
    DateTimeOffset OccurredAt) : IDomainEvent;