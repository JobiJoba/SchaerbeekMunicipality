using FluentValidation;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.RecordDeathFacts;

public sealed record RecordDeathFactsResponse(
    Guid CaseId,
    string Status,
    bool DeathFactsRecorded);

public sealed class RecordDeathFactsHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    IValidator<RecordDeathFactsRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<RecordDeathFactsResponse> Handle(
        DeathDeclarationCaseId caseId,
        RecordDeathFactsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordDeathFacts),
            cancellationToken);

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        deathDeclarationCase.RecordDeathFacts(
            new DeathFacts(
                request.DeathDate,
                request.DeathPlace,
                request.DeathAbroad,
                request.InformantRelationship),
            today);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordDeathFactsResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString(),
            deathDeclarationCase.Checklist.DeathFactsRecorded);
    }
}
