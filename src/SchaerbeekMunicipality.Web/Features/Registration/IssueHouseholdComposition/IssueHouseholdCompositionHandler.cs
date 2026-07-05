using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Household;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Infrastructure.Certificates;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Features.Registration.IssueResidenceCertificate;

namespace SchaerbeekMunicipality.Web.Features.Registration.IssueHouseholdComposition;

public sealed class IssueHouseholdCompositionHandler(
    RegistrationCaseGuard caseGuard,
    IPersonRepository personRepository,
    IHouseholdRepository householdRepository,
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

        if (registrationCase.Status != RegistrationCaseStatus.Registered)
        {
            throw new InvalidRegistrationTransitionException(
                "Certificates can only be issued for registered cases.");
        }

        if (registrationCase.PersonId is not { } personId)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before issuing a certificate.");
        }

        var person = await personRepository.GetByIdAsync(personId, cancellationToken)
            ?? throw new KeyNotFoundException($"Person '{personId}' was not found.");

        if (person.NationalRegisterNumber is null)
        {
            throw new InvalidRegistrationTransitionException(
                "A National Register number is required before issuing a certificate.");
        }

        var household = await householdRepository.GetByCaseIdAsync(caseId, cancellationToken);
        if (household is null || household.Members.Count == 0)
        {
            throw new InvalidRegistrationTransitionException(
                "Household composition must be recorded before issuing this certificate.");
        }

        var issuedAt = timeProvider.GetUtcNow();
        var referenceNumber = await CertificateReferenceNumberGenerator.NextAsync(
            certificateRepository,
            CertificateType.HouseholdComposition,
            cancellationToken);

        var certificate = CertificateRequest.Issue(
            caseId,
            CertificateType.HouseholdComposition,
            personId,
            OfficerId.From(currentOfficer.OfficerId),
            referenceNumber,
            issuedAt);

        await certificateRepository.AddAsync(certificate, cancellationToken);
        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CertificateIssued,
            $"Household composition {referenceNumber}",
            cancellationToken);
        await certificateRepository.SaveChangesAsync(cancellationToken);

        var members = household.Members
            .Select(m => new HouseholdMemberLine(
                $"{m.GivenName} {m.FamilyName}",
                m.Role.ToString(),
                m.BirthDate))
            .ToList();

        var html = certificateRenderer.RenderHouseholdComposition(new HouseholdCompositionCertificateData(
            referenceNumber,
            $"{person.GivenName} {person.FamilyName}",
            person.NationalRegisterNumber.Value.Format(),
            FormatAddress(registrationCase.DeclaredAddress),
            members,
            issuedAt));

        return new IssueCertificateResult(certificate.Id.Value, referenceNumber, html);
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
