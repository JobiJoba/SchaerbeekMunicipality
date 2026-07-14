using SchaerbeekMunicipality.Domain.Certificates;

namespace SchaerbeekMunicipality.Application.Features.Registration;

internal static class CertificateReferenceNumberGenerator
{
    public static async Task<string> NextAsync(
        ICertificateRequestRepository repository,
        CertificateType certificateType,
        CancellationToken cancellationToken)
    {
        var prefix = certificateType switch
        {
            CertificateType.ResidenceCertificate => "RC",
            CertificateType.HouseholdComposition => "HC",
            _ => "CERT"
        };

        var count = await repository.CountByTypeAsync(certificateType, cancellationToken);
        return $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D4}";
    }
}