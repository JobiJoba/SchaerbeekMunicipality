using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.DeclareReferenceAddress;

public sealed class DeclareReferenceAddressHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    CaseAuditRecorder auditRecorder)
{
    public async Task<DeclareReferenceAddressResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(DeclareReferenceAddress),
            cancellationToken);

        registrationCase.DeclareReferenceAddress();

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.AddressDeclared,
            "Reference address (no fixed abode).",
            cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new DeclareReferenceAddressResponse(
            registrationCase.Id.Value,
            registrationCase.Checklist.AddressDeclared,
            registrationCase.DeclaredAddress!.FormatSingleLine(),
            registrationCase.AddressDeclarationType.ToString());
    }
}
