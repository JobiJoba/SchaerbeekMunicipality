using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordPoliceResult;

public sealed class RecordPoliceResultHandler(
    IPoliceVerificationRepository policeVerificationRepository,
    IRegistrationCaseRepository registrationCaseRepository,
    IChangeOfAddressCaseRepository changeOfAddressCaseRepository,
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
                                  ?? throw new KeyNotFoundException(
                                      $"Police verification request '{requestId}' was not found.");

        if (!verificationRequest.IsPending)
            throw new InvalidPoliceVerificationException(
                "This police verification request has already been completed.");

        verificationRequest.RecordResult(
            request.Result,
            request.OfficerNotes,
            timeProvider.GetUtcNow());

        if (verificationRequest.RegistrationCaseId is { } registrationCaseId)
            return await HandleRegistrationCaseAsync(
                verificationRequest,
                registrationCaseId,
                request,
                cancellationToken);

        if (verificationRequest.ChangeOfAddressCaseId is { } changeOfAddressCaseId)
            return await HandleChangeOfAddressCaseAsync(
                verificationRequest,
                changeOfAddressCaseId,
                request,
                cancellationToken);

        throw new InvalidPoliceVerificationException(
            "Police verification request is not linked to a case.");
    }

    private async Task<RecordPoliceResultResponse> HandleRegistrationCaseAsync(
        PoliceVerificationRequest verificationRequest,
        RegistrationCaseId registrationCaseId,
        RecordPoliceResultRequest request,
        CancellationToken cancellationToken)
    {
        var registrationCase = await registrationCaseRepository.GetByIdAsync(registrationCaseId, cancellationToken)
                               ?? throw new KeyNotFoundException(
                                   $"Registration case '{registrationCaseId}' was not found.");

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

    private async Task<RecordPoliceResultResponse> HandleChangeOfAddressCaseAsync(
        PoliceVerificationRequest verificationRequest,
        ChangeOfAddressCaseId changeOfAddressCaseId,
        RecordPoliceResultRequest request,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await changeOfAddressCaseRepository.GetByIdAsync(
                                      changeOfAddressCaseId,
                                      cancellationToken)
                                  ?? throw new KeyNotFoundException(
                                      $"Change of address case '{changeOfAddressCaseId}' was not found.");

        changeOfAddressCase.ApplyPoliceVerificationResult(request.Result);

        await policeVerificationRepository.SaveChangesAsync(cancellationToken);

        return new RecordPoliceResultResponse(
            verificationRequest.Id.Value,
            changeOfAddressCase.Id.Value,
            request.Result,
            changeOfAddressCase.Checklist.PoliceVerificationPositive,
            changeOfAddressCase.Status.ToString(),
            "ChangeOfAddress");
    }
}