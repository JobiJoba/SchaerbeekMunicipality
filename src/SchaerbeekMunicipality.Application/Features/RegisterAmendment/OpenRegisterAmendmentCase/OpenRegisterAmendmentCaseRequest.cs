namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;

public sealed record OpenRegisterAmendmentCaseRequest(
    Guid PersonId,
    string AmendmentType);

public sealed record OpenRegisterAmendmentCaseResponse(
    Guid CaseId,
    Guid PersonId,
    string AmendmentType,
    string Status,
    DateTimeOffset OpenedAt);
