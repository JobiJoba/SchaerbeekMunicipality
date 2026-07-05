using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ApproveCase;

public sealed record ApproveCaseRequest(RegisterTarget RegisterTarget);

public sealed record ApproveCaseResponse(
    Guid CaseId,
    string Status,
    string RegisterTarget);
