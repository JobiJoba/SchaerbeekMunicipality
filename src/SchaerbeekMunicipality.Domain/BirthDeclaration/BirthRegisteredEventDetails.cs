using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public sealed record BirthRegisteredEventDetails(
    BirthDeclarationCaseId CaseId,
    PersonId ChildPersonId,
    string ChildNationalRegisterNumber,
    DateTimeOffset ConfirmedAt);
