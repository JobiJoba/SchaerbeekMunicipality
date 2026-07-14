namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ConfirmAddressChange;

public sealed record ConfirmAddressChangeResponse(
    Guid CaseId,
    string Status,
    Guid PersonId);