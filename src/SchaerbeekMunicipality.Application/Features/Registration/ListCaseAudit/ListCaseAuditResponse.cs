namespace SchaerbeekMunicipality.Application.Features.Registration.ListCaseAudit;

public sealed record CaseAuditEntryDto(
    Guid Id,
    string Action,
    Guid OfficerId,
    DateTimeOffset OccurredAt,
    string? Details);

public sealed record ListCaseAuditResponse(
    Guid CaseId,
    IReadOnlyList<CaseAuditEntryDto> Entries);