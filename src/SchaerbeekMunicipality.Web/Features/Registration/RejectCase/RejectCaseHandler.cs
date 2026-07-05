using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.RejectCase;

public sealed class RejectCaseHandler(
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectCaseRequest> validator)
{
    public async Task<RejectCaseResponse> Handle(
        RegistrationCaseId caseId,
        RejectCaseRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can reject registration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        var officer = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.Reject(
            officer,
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseRejected,
            $"{request.Reason}: {request.Notes}",
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            request.Reason.ToString());
    }
}
