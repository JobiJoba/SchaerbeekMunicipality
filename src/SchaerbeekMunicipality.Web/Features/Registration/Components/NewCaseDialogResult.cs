namespace SchaerbeekMunicipality.Web.Features.Registration.Components;

public sealed record NewCaseDialogResult(
    Guid CaseId,
    bool IsBirthDeclaration,
    bool IsChangeOfAddress = false,
    bool IsDocumentRequest = false,
    bool IsDeathDeclaration = false);