using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;
using SchaerbeekMunicipality.Infrastructure.Certificates;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.IssueResidenceCertificate;

public sealed record IssueCertificateResult(
    Guid CertificateId,
    string ReferenceNumber,
    string HtmlContent);

public sealed class IssueResidenceCertificateHandler(
    RegistrationCaseGuard caseGuard,
    IPersonRepository personRepository,
    ICertificateRequestRepository certificateRepository,
    ICertificateRenderer certificateRenderer,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<IssueCertificateResult> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        EnsureRegistered(registrationCase);

        if (registrationCase.PersonId is not { } personId)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before issuing a certificate.");
        }

        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        var nationalRegisterNumber = person.NationalRegisterNumber
            ?? throw new InvalidRegistrationTransitionException(
                "A National Register number is required before issuing a certificate.");

        var addressLine = FormatAddress(registrationCase.DeclaredAddress);
        var issuedAt = timeProvider.GetUtcNow();
        var referenceNumber = await CertificateReferenceNumberGenerator.NextAsync(
            certificateRepository,
            CertificateType.ResidenceCertificate,
            cancellationToken);

        var certificate = CertificateRequest.Issue(
            caseId,
            CertificateType.ResidenceCertificate,
            personId,
            OfficerId.From(currentOfficer.OfficerId),
            referenceNumber,
            issuedAt);

        await certificateRepository.AddAsync(certificate, cancellationToken);
        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CertificateIssued,
            $"Residence certificate {referenceNumber}",
            cancellationToken);
        await certificateRepository.SaveChangesAsync(cancellationToken);

        var html = certificateRenderer.RenderResidenceCertificate(new ResidenceCertificateData(
            referenceNumber,
            $"{person.GivenName} {person.FamilyName}",
            nationalRegisterNumber.Format(),
            addressLine,
            registrationCase.SelectedRegisterTarget?.ToString() ?? "Unknown",
            issuedAt,
            registrationCase.ClosedAt));

        return new IssueCertificateResult(certificate.Id.Value, referenceNumber, html);
    }

    private static void EnsureRegistered(RegistrationCase registrationCase)
    {
        if (registrationCase.Status != RegistrationCaseStatus.Registered)
        {
            throw new InvalidRegistrationTransitionException(
                "Certificates can only be issued for registered cases.");
        }
    }

    private static string FormatAddress(BelgianAddress? address)
    {
        if (address is null)
        {
            return "Address not recorded";
        }

        var box = string.IsNullOrWhiteSpace(address.Box) ? string.Empty : $" bte {address.Box}";
        return $"{address.Street} {address.HouseNumber}{box}, {address.PostalCode} {address.Municipality}";
    }
}
