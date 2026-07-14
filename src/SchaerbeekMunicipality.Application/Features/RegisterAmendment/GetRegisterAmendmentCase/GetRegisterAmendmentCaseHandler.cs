using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.RegisterAmendment;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.RegisterAmendment.GetRegisterAmendmentCase;

public sealed record RegisterAmendmentDocumentDto(
    Guid Id,
    string DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record RegisterAmendmentCaseDetailDto(
    Guid CaseId,
    RegisterAmendmentCaseStatus Status,
    AmendmentType AmendmentType,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool CanEdit,
    bool IsReadOnlyDueToLock,
    Guid PersonId,
    string PersonGivenName,
    string PersonFamilyName,
    string? PersonNationalRegisterNumber,
    string? PersonNationality,
    string? CurrentGivenName,
    string? CurrentFamilyName,
    string? CurrentNationality,
    CivilStatusDto? CurrentCivilStatus,
    string? Reason,
    string? ProposedGivenName,
    string? ProposedFamilyName,
    string? ProposedNationality,
    CivilStatusDto? ProposedCivilStatus,
    bool ProposedChangesRecorded,
    bool SupportingDocumentAttached,
    bool IsReadyForApproval,
    DateTimeOffset OpenedAt,
    DateTimeOffset? SubmittedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? AppliedAt,
    DateTimeOffset? ClosedAt,
    RegisterAmendmentRejectionReason? RejectionReason,
    string? DecisionNotes,
    IReadOnlyList<RegisterAmendmentDocumentDto> Documents);

public sealed class GetRegisterAmendmentCaseHandler(
    RegisterAmendmentCaseGuard caseGuard,
    RegisterAmendmentCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository)
{
    public async Task<RegisterAmendmentCaseDetailDto?> Handle(
        RegisterAmendmentCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var amendmentCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);
        var person = await personRepository.GetByIdAsync(amendmentCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{amendmentCase.PersonId}' was not found.");

        var documents = await documentRepository.ListByRegisterAmendmentCaseIdAsync(caseId, cancellationToken);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, amendmentCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            amendmentCase,
            officerId);

        return new RegisterAmendmentCaseDetailDto(
            amendmentCase.Id.Value,
            amendmentCase.Status,
            amendmentCase.AmendmentType,
            amendmentCase.AssignedOfficerId?.Value,
            amendmentCase.LockedByOfficerId?.Value,
            amendmentCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            person.NationalRegisterNumber?.Value,
            person.Nationality,
            person.GivenName,
            person.FamilyName,
            person.Nationality,
            MapCivilStatus(person.CivilStatus),
            amendmentCase.Reason,
            amendmentCase.ProposedGivenName,
            amendmentCase.ProposedFamilyName,
            amendmentCase.ProposedNationality,
            MapCivilStatus(amendmentCase.ProposedCivilStatus),
            amendmentCase.Checklist.ProposedChangesRecorded,
            amendmentCase.Checklist.SupportingDocumentAttached,
            amendmentCase.IsReadyForApproval,
            amendmentCase.OpenedAt,
            amendmentCase.SubmittedAt,
            amendmentCase.ApprovedAt,
            amendmentCase.AppliedAt,
            amendmentCase.ClosedAt,
            amendmentCase.RejectionReason,
            amendmentCase.DecisionNotes,
            documents.Select(d => new RegisterAmendmentDocumentDto(
                d.Id.Value,
                d.DocumentType.ToString(),
                d.FileName,
                d.UploadedAt)).ToList());
    }

    private static CivilStatusDto? MapCivilStatus(CivilStatusRecord? civilStatus)
    {
        if (civilStatus is null) return null;

        return new CivilStatusDto(
            civilStatus.Status,
            civilStatus.EffectiveRegisterStatus,
            civilStatus.SpouseGivenName,
            civilStatus.SpouseFamilyName,
            civilStatus.MarriageDate,
            civilStatus.MarriagePlace,
            civilStatus.MarriageRecognitionStatus);
    }
}
