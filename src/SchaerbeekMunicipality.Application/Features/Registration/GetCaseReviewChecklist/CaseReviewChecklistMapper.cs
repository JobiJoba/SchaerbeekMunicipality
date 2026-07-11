using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;

public static class CaseReviewChecklistMapper
{
    public static GetCaseReviewChecklistResponse FromCaseDetail(RegistrationCaseDetailDto caseDetail) =>
        new(
            caseDetail.Id,
            caseDetail.Status.ToString(),
            caseDetail.IsReadyForApproval,
            caseDetail.SuggestedRegisterTarget,
            BuildQuestions(caseDetail));

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(RegistrationCaseChecklistDto checklist) =>
        BuildQuestions(
            checklist.IdentityEstablished,
            checklist.LegalResidenceEstablished,
            checklist.AddressDeclared,
            checklist.AddressConfirmed,
            checklist.RegisterDeterminable,
            checklist.BirthEvidenceEstablished,
            checklist.DuplicateInvestigationResolved);

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(RegistrationCaseChecklist checklist) =>
        BuildQuestions(
            checklist.IdentityEstablished,
            checklist.LegalResidenceEstablished,
            checklist.AddressDeclared,
            checklist.AddressConfirmed,
            checklist.RegisterDeterminable,
            checklist.BirthEvidenceEstablished,
            checklist.DuplicateInvestigationResolved);

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(RegistrationCaseDetailDto caseDetail) =>
        BuildQuestions(
            caseDetail.Checklist.IdentityEstablished,
            caseDetail.Checklist.LegalResidenceEstablished,
            caseDetail.Checklist.AddressDeclared,
            caseDetail.Checklist.AddressConfirmed,
            caseDetail.Checklist.RegisterDeterminable,
            caseDetail.Checklist.BirthEvidenceEstablished,
            caseDetail.Checklist.DuplicateInvestigationResolved,
            caseDetail.MarriageRecognitionBlocking,
            caseDetail.IllegalStayDetected);

    public static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(
        RegistrationCaseChecklist checklist,
        bool birthEvidenceEstablished,
        bool marriageRecognitionBlocking,
        bool illegalStayDetected) =>
        BuildQuestions(
            checklist.IdentityEstablished,
            checklist.LegalResidenceEstablished,
            checklist.AddressDeclared,
            checklist.AddressConfirmed,
            checklist.RegisterDeterminable,
            birthEvidenceEstablished,
            checklist.DuplicateInvestigationResolved,
            marriageRecognitionBlocking,
            illegalStayDetected);

    private static IReadOnlyList<CaseReviewQuestionDto> BuildQuestions(
        bool identityEstablished,
        bool legalResidenceEstablished,
        bool addressDeclared,
        bool addressConfirmed,
        bool registerDeterminable,
        bool birthEvidenceEstablished,
        bool duplicateInvestigationResolved,
        bool marriageRecognitionBlocking = false,
        bool illegalStayDetected = false) =>
    [
        new(
            "identity",
            "Is identity established with sufficient certainty?",
            identityEstablished,
            identityEstablished ? null : "Record and verify identity documents."),
        new(
            "legal-residence",
            "Is there a legal basis to reside in Belgium?",
            legalResidenceEstablished && !illegalStayDetected,
            illegalStayDetected
                ? "No legal residence basis — reject and refer to Immigration Office."
                : legalResidenceEstablished
                    ? null
                    : "Set residence category and satisfy permit policy."),
        new(
            "birth-evidence",
            "Is birth information complete and certificate on file?",
            birthEvidenceEstablished,
            birthEvidenceEstablished
                ? null
                : "Record birth place and attach a birth certificate, or suspend until documents arrive."),
        new(
            "duplicate-investigation",
            "Has duplicate identity investigation been resolved?",
            duplicateInvestigationResolved,
            duplicateInvestigationResolved
                ? null
                : "Review National Register matches and link or confirm a distinct person."),
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
            registerDeterminable && !marriageRecognitionBlocking,
            marriageRecognitionBlocking
                ? "Foreign marriage recognition is pending — suspend until recognised."
                : registerDeterminable
                    ? null
                    : "Set residence category to determine register target."),
    ];
}
