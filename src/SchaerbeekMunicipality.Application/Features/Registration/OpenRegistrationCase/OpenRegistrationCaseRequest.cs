using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;

public sealed record OpenRegistrationCaseRequest(
    VisitReason VisitReason,
    Guid? AssignedOfficerId);

public sealed record OpenRegistrationCaseResponse(
    Guid CaseId,
    string Status,
    VisitReason VisitReason,
    DateTimeOffset OpenedAt);
