namespace SchaerbeekMunicipality.Web.Features.Registration.RecordBirthInformation;

public sealed record RecordBirthInformationRequest(
    string BirthPlace,
    string? BirthCountry);

public sealed record RecordBirthInformationResponse(
    Guid CaseId,
    string BirthPlace,
    string? BirthCountry,
    bool BirthEvidenceEstablished);
