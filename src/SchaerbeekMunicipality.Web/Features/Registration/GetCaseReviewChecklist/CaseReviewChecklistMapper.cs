using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetCaseReviewChecklist;

public static class CaseReviewChecklistMapper
{
    public static GetCaseReviewChecklistResponse FromCaseDetail(RegistrationCaseDetailDto caseDetail) =>
        new(
            caseDetail.Id,
            caseDetail.Status.ToString(),
            caseDetail.IsReadyForApproval,
            caseDetail.SuggestedRegisterTarget,
            BuildQuestions(caseDetail.Checklist));

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(RegistrationCaseChecklistDto checklist) =>
        BuildQuestions(
            checklist.IdentityEstablished,
            checklist.LegalResidenceEstablished,
            checklist.AddressDeclared,
            checklist.AddressConfirmed,
            checklist.RegisterDeterminable);

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(RegistrationCaseChecklist checklist) =>
        BuildQuestions(
            checklist.IdentityEstablished,
            checklist.LegalResidenceEstablished,
            checklist.AddressDeclared,
            checklist.AddressConfirmed,
            checklist.RegisterDeterminable);

    private static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(
        bool identityEstablished,
        bool legalResidenceEstablished,
        bool addressDeclared,
        bool addressConfirmed,
        bool registerDeterminable) =>
    [
        new(
            "identity",
            "Is identity established with sufficient certainty?",
            identityEstablished,
            identityEstablished ? null : "Record and verify identity documents."),
        new(
            "legal-residence",
            "Is there a legal basis to reside in Belgium?",
            legalResidenceEstablished,
            legalResidenceEstablished ? null : "Set residence category and satisfy permit policy."),
        new(
            "address",
            "Is residence at the declared address genuine?",
            addressConfirmed,
            addressConfirmed
                ? null
                : addressDeclared
                    ? "Await positive police verification result."
                    : "Declare address and complete police verification."),
        new(
            "register",
            "Is the correct register determinable?",
            registerDeterminable,
            registerDeterminable ? null : "Set residence category to determine register target."),
    ];
}
