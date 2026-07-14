namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;

public sealed record OpenChangeOfAddressCaseResponse(
    Guid CaseId,
    Guid PersonId,
    string Status,
    DateTimeOffset OpenedAt);