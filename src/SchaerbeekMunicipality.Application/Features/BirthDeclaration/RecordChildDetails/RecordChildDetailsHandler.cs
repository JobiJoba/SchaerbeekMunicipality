using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;

public sealed record RecordChildDetailsResponse(
    Guid CaseId,
    string Status,
    bool ChildDetailsRecorded);

public sealed class RecordChildDetailsHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    IValidator<RecordChildDetailsRequest> validator,
    TimeProvider timeProvider)
{
    public async Task<RecordChildDetailsResponse> Handle(
        BirthDeclarationCaseId caseId,
        RecordChildDetailsRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordChildDetails),
            cancellationToken);

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        birthDeclarationCase.RecordChildDetails(
            new NewbornDetails(
                request.GivenNames,
                request.FamilyName,
                request.Sex,
                request.DateOfBirth,
                request.TimeOfBirth,
                request.PlaceOfBirth),
            today);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordChildDetailsResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString(),
            birthDeclarationCase.Checklist.ChildDetailsRecorded);
    }
}
