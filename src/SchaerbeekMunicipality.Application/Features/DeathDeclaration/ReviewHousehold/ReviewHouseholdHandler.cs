using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReviewHousehold;

public sealed record ReviewHouseholdResponse(Guid CaseId, string Status, bool HouseholdReviewed);

public sealed class ReviewHouseholdHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    TimeProvider timeProvider)
{
    public async Task<ReviewHouseholdResponse> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ReviewHousehold),
            cancellationToken);

        deathDeclarationCase.ReviewHousehold(timeProvider.GetUtcNow());
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ReviewHouseholdResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString(),
            deathDeclarationCase.Checklist.HouseholdReviewed);
    }
}
