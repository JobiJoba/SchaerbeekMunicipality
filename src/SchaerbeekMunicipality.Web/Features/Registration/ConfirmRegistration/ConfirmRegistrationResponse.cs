namespace SchaerbeekMunicipality.Web.Features.Registration.ConfirmRegistration;

public sealed record ConfirmRegistrationResponse(
    Guid CaseId,
    string Status,
    string RegisterTarget,
    string? NationalRegisterNumber);
