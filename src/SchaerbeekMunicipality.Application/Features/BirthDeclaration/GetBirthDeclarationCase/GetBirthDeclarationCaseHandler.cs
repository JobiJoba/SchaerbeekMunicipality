using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;

public sealed class GetBirthDeclarationCaseHandler(
    BirthDeclarationCaseGuard caseGuard,
    BirthDeclarationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository)
{
    public async Task<BirthDeclarationCaseDetailDto?> Handle(
        BirthDeclarationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var birthDeclarationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);
        var documents = await documentRepository.ListByBirthDeclarationCaseIdAsync(caseId, cancellationToken);

        var parents = new List<BirthDeclarationParentDto>();
        foreach (var link in birthDeclarationCase.ParentLinks)
        {
            var person = await personRepository.GetByIdAsync(link.PersonId, cancellationToken);
            if (person is null)
            {
                continue;
            }

            parents.Add(new BirthDeclarationParentDto(
                person.Id.Value,
                person.GivenName,
                person.FamilyName,
                person.NationalRegisterNumber?.Value,
                link.Role));
        }

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, birthDeclarationCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            birthDeclarationCase,
            officerId);

        var checklist = birthDeclarationCase.Checklist;

        return new BirthDeclarationCaseDetailDto(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status,
            birthDeclarationCase.AssignedOfficerId?.Value,
            birthDeclarationCase.LockedByOfficerId?.Value,
            birthDeclarationCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            birthDeclarationCase.ChildGivenNames,
            birthDeclarationCase.ChildFamilyName,
            birthDeclarationCase.ChildSex,
            birthDeclarationCase.ChildDateOfBirth,
            birthDeclarationCase.ChildTimeOfBirth,
            birthDeclarationCase.ChildPlaceOfBirth,
            parents,
            documents.Select(d => new BirthDeclarationDocumentDto(
                d.Id.Value,
                d.DocumentType,
                d.FileName,
                d.UploadedAt)).ToList(),
            birthDeclarationCase.HouseholdAddress?.FormatSingleLine(),
            checklist.ChildDetailsRecorded,
            checklist.AtLeastOneParentLinked,
            checklist.MedicalDeclarationAttached,
            checklist.HouseholdEstablished,
            birthDeclarationCase.IsReadyForConfirmation,
            birthDeclarationCase.OpenedAt,
            birthDeclarationCase.ConfirmedAt,
            birthDeclarationCase.ChildNationalRegisterNumber,
            birthDeclarationCase.RejectionReason,
            birthDeclarationCase.SuspensionReason,
            birthDeclarationCase.DecisionNotes);
    }
}
