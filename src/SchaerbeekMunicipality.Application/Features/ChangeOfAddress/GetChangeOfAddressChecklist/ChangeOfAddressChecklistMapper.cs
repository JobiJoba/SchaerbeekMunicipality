using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressChecklist;

public sealed record ChangeOfAddressChecklistQuestion(
    string Question,
    bool IsSatisfied,
    string? BlockingReason);

public sealed record ChangeOfAddressChecklistResponse(
    bool ReadyForConfirmation,
    IReadOnlyList<ChangeOfAddressChecklistQuestion> Questions);

public static class ChangeOfAddressChecklistMapper
{
    public static ChangeOfAddressChecklistResponse FromCaseDetail(ChangeOfAddressCaseDetailDto detail) =>
        new(
            detail.ReadyForConfirmation,
            [
                new(
                    "Person identified in the National Register",
                    detail.PersonIdentified,
                    detail.PersonIdentified ? null : "Open the case for a registered person."),
                new(
                    "New Schaerbeek address declared",
                    detail.NewAddressDeclared,
                    detail.NewAddressDeclared ? null : "Declare the new domicile address."),
                new(
                    detail.HousingDocumentRequired
                        ? "Housing document attached (rental contract)"
                        : "Housing document (not required for this housing situation)",
                    !detail.HousingDocumentRequired || detail.HousingDocumentAttached,
                    detail.HousingDocumentRequired && !detail.HousingDocumentAttached
                        ? "Attach the rental contract or other housing proof."
                        : null),
                new(
                    detail.PoliceVerificationRequested
                        ? "Police verification completed positively"
                        : "Police verification (optional unless requested)",
                    !detail.PoliceVerificationRequested || detail.PoliceVerificationPositive,
                    detail.PoliceVerificationRequested && !detail.PoliceVerificationPositive
                        ? "Await a positive police verification result."
                        : null),
            ]);
}
