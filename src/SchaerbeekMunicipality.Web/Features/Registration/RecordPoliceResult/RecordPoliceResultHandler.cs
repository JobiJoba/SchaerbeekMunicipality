using FluentValidation;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;

public sealed class RecordPoliceResultHandler(
    IPoliceVerificationRepository policeVerificationRepository,
    IRegistrationCaseRepository caseRepository,
    RegistrationCaseAuthorization authorization,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    IValidator<RecordPoliceResultRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<RecordPoliceResultResponse> Handle(
        PoliceVerificationRequestId requestId,
        RecordPoliceResultRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanRecordPoliceResult(currentOfficer);
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var verificationRequest = await policeVerificationRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Police verification request '{requestId}' was not found.");

        if (!verificationRequest.IsPending)
        {
            throw new InvalidPoliceVerificationException(
                "This police verification request has already been completed.");
        }

        var registrationCase = await caseRepository.GetByIdAsync(
                verificationRequest.RegistrationCaseId,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Registration case '{verificationRequest.RegistrationCaseId}' was not found.");

        verificationRequest.RecordResult(
            request.Result,
            request.OfficerNotes,
            timeProvider.GetUtcNow());

        registrationCase.ApplyPoliceVerificationResult(request.Result);

        await auditRecorder.RecordAsync(
            registrationCase.Id,
            CaseAuditAction.PoliceResultRecorded,
            request.Result.ToString(),
            cancellationToken);

        await policeVerificationRepository.SaveChangesAsync(cancellationToken);

        return new RecordPoliceResultResponse(
            verificationRequest.Id.Value,
            registrationCase.Id.Value,
            request.Result,
            registrationCase.Checklist.AddressConfirmed,
            registrationCase.Status.ToString());
    }
}
