namespace SchaerbeekMunicipality.Infrastructure.Certificates;

public sealed record ResidenceCertificateData(
    string ReferenceNumber,
    string PersonName,
    string NationalRegisterNumber,
    string AddressLine,
    string RegisterTarget,
    DateTimeOffset IssuedAt,
    DateTimeOffset? RegisteredAt);

public sealed record HouseholdMemberLine(
    string Name,
    string Role,
    DateOnly BirthDate);

public sealed record HouseholdCompositionCertificateData(
    string ReferenceNumber,
    string PersonName,
    string NationalRegisterNumber,
    string AddressLine,
    IReadOnlyList<HouseholdMemberLine> Members,
    DateTimeOffset IssuedAt);

public interface ICertificateRenderer
{
    string RenderResidenceCertificate(ResidenceCertificateData data);

    string RenderHouseholdComposition(HouseholdCompositionCertificateData data);
}