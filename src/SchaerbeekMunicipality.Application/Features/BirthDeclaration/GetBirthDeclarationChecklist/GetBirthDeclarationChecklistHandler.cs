using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationChecklist;

public sealed record BirthDeclarationChecklistQuestion(
    string Question,
    bool IsSatisfied,
    string? BlockingReason);

public sealed record BirthDeclarationChecklistResponse(
    bool ReadyForConfirmation,
    IReadOnlyList<BirthDeclarationChecklistQuestion> Questions);

public static class BirthDeclarationChecklistMapper
{
    public static BirthDeclarationChecklistResponse FromCaseDetail(BirthDeclarationCaseDetailDto detail) =>
        new(
            detail.ReadyForConfirmation,
            [
                new(
                    "Child identity recorded (names, sex, date and place of birth)",
                    detail.ChildDetailsRecorded,
                    detail.ChildDetailsRecorded ? null : "Record the newborn details."),
                new(
                    "At least one parent linked in the National Register",
                    detail.AtLeastOneParentLinked,
                    detail.AtLeastOneParentLinked ? null : "Link a registered parent via NR search."),
                new(
                    "Medical birth declaration attached",
                    detail.MedicalDeclarationAttached,
                    detail.MedicalDeclarationAttached ? null : "Attach the hospital or midwife document."),
                new(
                    "Household domicile established",
                    detail.HouseholdEstablished,
                    detail.HouseholdEstablished ? null : "Declare the parents' domicile address."),
            ]);
}

public sealed class GetBirthDeclarationChecklistHandler(
    GetBirthDeclarationCaseHandler getCaseHandler)
{
    public async Task<BirthDeclarationChecklistResponse?> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var detail = await getCaseHandler.Handle(caseId, cancellationToken);
        return detail is null ? null : BirthDeclarationChecklistMapper.FromCaseDetail(detail);
    }
}
