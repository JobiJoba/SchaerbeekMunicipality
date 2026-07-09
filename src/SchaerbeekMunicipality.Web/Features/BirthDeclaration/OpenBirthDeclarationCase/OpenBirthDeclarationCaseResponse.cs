namespace SchaerbeekMunicipality.Web.Features.BirthDeclaration.OpenBirthDeclarationCase;

public sealed record OpenBirthDeclarationCaseResponse(
    Guid CaseId,
    string Status,
    DateTimeOffset OpenedAt);
