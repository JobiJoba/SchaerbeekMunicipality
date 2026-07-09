using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Events;

public sealed record BirthRegistered(
    BirthDeclarationCaseId CaseId,
    PersonId ChildPersonId,
    string ChildNationalRegisterNumber,
    DateTimeOffset OccurredAt) : IDomainEvent;
