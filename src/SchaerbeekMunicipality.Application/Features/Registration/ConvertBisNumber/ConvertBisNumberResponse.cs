namespace SchaerbeekMunicipality.Application.Features.Registration.ConvertBisNumber;

public sealed record ConvertBisNumberResponse(
    Guid CaseId,
    Guid PersonId,
    string BisNumber,
    string NationalRegisterNumber);
