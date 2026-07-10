using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public sealed record AddressChangedEventDetails(
    ChangeOfAddressCaseId CaseId,
    PersonId PersonId,
    DateTimeOffset ConfirmedAt);
