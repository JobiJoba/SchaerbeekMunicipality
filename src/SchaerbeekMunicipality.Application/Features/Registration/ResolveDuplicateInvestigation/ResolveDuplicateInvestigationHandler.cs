using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.ResolveDuplicateInvestigation;

public sealed class ResolveDuplicateInvestigationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    IValidator<ResolveDuplicateInvestigationRequest> validator)
{
    public async Task<ResolveDuplicateInvestigationResponse> Handle(
        RegistrationCaseId caseId,
        ResolveDuplicateInvestigationRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ResolveDuplicateInvestigation),
            cancellationToken);

        var officer = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.ResolveDuplicateAsDistinct(officer, request.Notes);

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.DuplicateInvestigationResolved,
            request.Notes ?? "Confirmed distinct person after investigation.",
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ResolveDuplicateInvestigationResponse(
            caseId.Value,
            registrationCase.DuplicateInvestigationStatus.ToString(),
            registrationCase.Checklist.DuplicateInvestigationResolved);
    }
}