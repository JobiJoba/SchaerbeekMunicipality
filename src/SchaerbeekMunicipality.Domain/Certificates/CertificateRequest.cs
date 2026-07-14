using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Certificates;

public sealed class CertificateRequest
{
    private CertificateRequest()
    {
    }

    public CertificateRequestId Id { get; private set; }

    public RegistrationCaseId RegistrationCaseId { get; private set; }

    public CertificateType CertificateType { get; private set; }

    public PersonId PersonId { get; private set; }

    public OfficerId IssuedByOfficerId { get; private set; }

    public string ReferenceNumber { get; private set; } = string.Empty;

    public DateTimeOffset IssuedAt { get; private set; }

    public static CertificateRequest Issue(
        RegistrationCaseId registrationCaseId,
        CertificateType certificateType,
        PersonId personId,
        OfficerId issuedByOfficerId,
        string referenceNumber,
        DateTimeOffset issuedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(referenceNumber);

        return new CertificateRequest
        {
            Id = CertificateRequestId.New(),
            RegistrationCaseId = registrationCaseId,
            CertificateType = certificateType,
            PersonId = personId,
            IssuedByOfficerId = issuedByOfficerId,
            ReferenceNumber = referenceNumber.Trim(),
            IssuedAt = issuedAt
        };
    }
}