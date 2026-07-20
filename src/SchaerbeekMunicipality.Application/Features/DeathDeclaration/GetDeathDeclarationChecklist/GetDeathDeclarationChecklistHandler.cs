using SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationCase;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationChecklist;

public sealed record DeathDeclarationChecklistQuestion(
    string Question,
    bool IsSatisfied,
    string? BlockingReason);

public sealed record DeathDeclarationChecklistResponse(
    bool ReadyForConfirmation,
    IReadOnlyList<DeathDeclarationChecklistQuestion> Questions);

public static class DeathDeclarationChecklistMapper
{
    public static DeathDeclarationChecklistResponse FromCaseDetail(DeathDeclarationCaseDetailDto detail)
    {
        return new DeathDeclarationChecklistResponse(
            detail.ReadyForConfirmation,
            [
                new DeathDeclarationChecklistQuestion(
                    "Deceased person identified in the register",
                    detail.PersonIdentified,
                    detail.PersonIdentified ? null : "Resolve the registered person via NR search."),
                new DeathDeclarationChecklistQuestion(
                    "Death facts recorded (date, place, informant)",
                    detail.DeathFactsRecorded,
                    detail.DeathFactsRecorded ? null : "Record the death date, place and informant relationship."),
                new DeathDeclarationChecklistQuestion(
                    "Death act attached",
                    detail.DeathActAttached,
                    detail.DeathActAttached ? null : "Attach the death certificate document."),
                new DeathDeclarationChecklistQuestion(
                    "Household reviewed",
                    detail.HouseholdReviewed,
                    detail.HouseholdReviewed ? null : "Review the household composition before radiation.")
            ]);
    }
}

public sealed class GetDeathDeclarationChecklistHandler(
    GetDeathDeclarationCaseHandler getCaseHandler)
{
    public async Task<DeathDeclarationChecklistResponse?> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var detail = await getCaseHandler.Handle(caseId, cancellationToken);
        return detail is null ? null : DeathDeclarationChecklistMapper.FromCaseDetail(detail);
    }
}
