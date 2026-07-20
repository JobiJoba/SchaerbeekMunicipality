using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationCase;

public sealed class GetDeathDeclarationCaseHandler(
    DeathDeclarationCaseGuard caseGuard,
    DeathDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository)
{
    public async Task<DeathDeclarationCaseDetailDto?> Handle(
        DeathDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var deathDeclarationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);
        var documents = await documentRepository.ListByDeathDeclarationCaseIdAsync(caseId, cancellationToken);
        var person = await personRepository.GetByIdAsync(deathDeclarationCase.PersonId, cancellationToken);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, deathDeclarationCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            deathDeclarationCase,
            officerId);

        var checklist = deathDeclarationCase.Checklist;

        return new DeathDeclarationCaseDetailDto(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status,
            deathDeclarationCase.AssignedOfficerId?.Value,
            deathDeclarationCase.LockedByOfficerId?.Value,
            deathDeclarationCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            deathDeclarationCase.PersonId.Value,
            person?.GivenName,
            person?.FamilyName,
            person?.NationalRegisterNumber?.Value,
            person?.BirthDate,
            deathDeclarationCase.DeathDate,
            deathDeclarationCase.DeathPlace,
            deathDeclarationCase.DeathAbroad,
            deathDeclarationCase.InformantRelationship,
            documents.Select(d => new DeathDeclarationDocumentDto(
                d.Id.Value,
                d.DocumentType,
                d.FileName,
                d.UploadedAt)).ToList(),
            deathDeclarationCase.HouseholdReviewedAt,
            checklist.PersonIdentified,
            checklist.DeathFactsRecorded,
            checklist.DeathActAttached,
            checklist.HouseholdReviewed,
            deathDeclarationCase.IsReadyForConfirmation,
            deathDeclarationCase.OpenedAt,
            deathDeclarationCase.ConfirmedAt,
            deathDeclarationCase.RejectionReason,
            deathDeclarationCase.SuspensionReason,
            deathDeclarationCase.DecisionNotes);
    }
}
