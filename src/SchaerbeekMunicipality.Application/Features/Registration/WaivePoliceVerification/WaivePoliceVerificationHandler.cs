using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.WaivePoliceVerification;

public sealed class WaivePoliceVerificationHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder)
{
    public async Task<WaivePoliceVerificationResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(WaivePoliceVerification),
            cancellationToken);

        registrationCase.WaivePoliceVerification();

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.PoliceVerificationWaived,
            "Diplomat police verification waived (special register protocol).",
            cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new WaivePoliceVerificationResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            registrationCase.Checklist.AddressConfirmed);
    }
}
