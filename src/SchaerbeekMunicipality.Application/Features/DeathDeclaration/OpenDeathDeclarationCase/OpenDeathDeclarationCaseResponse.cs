namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;

public sealed record OpenDeathDeclarationCaseResponse(
    Guid CaseId,
    Guid PersonId,
    string Status,
    DateTimeOffset OpenedAt);
