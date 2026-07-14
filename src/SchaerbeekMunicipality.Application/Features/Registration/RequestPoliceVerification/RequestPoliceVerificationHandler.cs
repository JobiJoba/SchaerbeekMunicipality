using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RequestPoliceVerification;

public sealed class RequestPoliceVerificationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPoliceVerificationRepository policeVerificationRepository,
    CaseAuditRecorder auditRecorder,
    TimeProvider timeProvider)
{
    public async Task<RequestPoliceVerificationResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RequestPoliceVerification),
            cancellationToken);

        var pending = await policeVerificationRepository.GetPendingByCaseIdAsync(caseId, cancellationToken);
        if (pending is not null)
            throw new InvalidPoliceVerificationException(
                "A police verification request is already pending for this case.");

        registrationCase.RequestPoliceVerification();

        var maxAttempt = await policeVerificationRepository.GetMaxAttemptNumberAsync(caseId, cancellationToken);
        var request = PoliceVerificationRequest.Create(
            caseId,
            maxAttempt + 1,
            timeProvider.GetUtcNow());

        await policeVerificationRepository.AddAsync(request, cancellationToken);
        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.PoliceVerificationRequested,
            $"Attempt {request.AttemptNumber}",
            cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RequestPoliceVerificationResponse(
            registrationCase.Id.Value,
            request.Id.Value,
            request.AttemptNumber,
            registrationCase.Status.ToString());
    }
}