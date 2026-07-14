using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.IdentityDocuments.GetDocumentRequestCase;

public sealed record DocumentRequestDocumentDto(
    Guid Id,
    DocumentType DocumentType,
    string FileName,
    DateTimeOffset UploadedAt);

public sealed record DocumentRequestCaseDetailDto(
    Guid Id,
    DocumentRequestCaseStatus Status,
    DocumentRequestType RequestType,
    Guid? AssignedOfficerId,
    Guid? LockedByOfficerId,
    DateTimeOffset? LockedAt,
    bool CanEdit,
    bool IsReadOnlyDueToLock,
    Guid PersonId,
    string GivenName,
    string FamilyName,
    string? NationalRegisterNumber,
    DateOnly BirthDate,
    bool PhotoAttached,
    bool FeePaid,
    string? FeePaymentReference,
    string? IssuedDocumentNumber,
    DateTimeOffset RequestedAt,
    DateTimeOffset? IssuedAt,
    DateTimeOffset? CancelledAt,
    string? CancellationReason,
    IReadOnlyList<DocumentRequestDocumentDto> Documents);

public sealed class GetDocumentRequestCaseHandler(
    DocumentRequestCaseGuard caseGuard,
    DocumentRequestCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository)
{
    public async Task<DocumentRequestCaseDetailDto?> Handle(
        DocumentRequestCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var documentRequestCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);
        var person = await personRepository.GetByIdAsync(documentRequestCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{documentRequestCase.PersonId}' was not found.");

        var documents = await documentRepository.ListByDocumentRequestCaseIdAsync(caseId, cancellationToken);

        var officerId = OfficerId.From(currentOfficer.OfficerId);
        var canEdit = authorization.CanEditCase(currentOfficer.Role, documentRequestCase, officerId);
        var isReadOnlyDueToLock = authorization.IsReadOnlyDueToLock(
            currentOfficer.Role,
            documentRequestCase,
            officerId);

        return new DocumentRequestCaseDetailDto(
            documentRequestCase.Id.Value,
            documentRequestCase.Status,
            documentRequestCase.RequestType,
            documentRequestCase.AssignedOfficerId?.Value,
            documentRequestCase.LockedByOfficerId?.Value,
            documentRequestCase.LockedAt,
            canEdit,
            isReadOnlyDueToLock,
            person.Id.Value,
            person.GivenName,
            person.FamilyName,
            person.NationalRegisterNumber?.Value,
            person.BirthDate,
            documentRequestCase.PhotoAttached,
            documentRequestCase.FeePaid,
            documentRequestCase.FeePaymentReference,
            documentRequestCase.IssuedDocumentNumber,
            documentRequestCase.RequestedAt,
            documentRequestCase.IssuedAt,
            documentRequestCase.CancelledAt,
            documentRequestCase.CancellationReason,
            documents.Select(d => new DocumentRequestDocumentDto(
                d.Id.Value,
                d.DocumentType,
                d.FileName,
                d.UploadedAt)).ToList());
    }
}