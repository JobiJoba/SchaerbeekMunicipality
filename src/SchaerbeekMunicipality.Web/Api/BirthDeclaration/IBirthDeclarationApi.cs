using SchaerbeekMunicipality.Application.Features.BirthDeclaration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ClaimBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ConfirmBirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.GetBirthDeclarationChecklist;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.LinkParent;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ListBirthDeclarationCases;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RejectBirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RemoveDocument;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ResumeBirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.SuspendBirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.UnlinkParent;

namespace SchaerbeekMunicipality.Web.Api.BirthDeclaration;

public interface IBirthDeclarationApi
{
    Task<IReadOnlyList<BirthDeclarationCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default);

    Task<OpenBirthDeclarationCaseResponse> OpenCaseAsync(CancellationToken cancellationToken = default);

    Task<BirthDeclarationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimBirthDeclarationCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecordChildDetailsResponse> RecordChildDetailsAsync(
        Guid id,
        RecordChildDetailsRequest request,
        CancellationToken cancellationToken = default);

    Task<LinkParentResponse> LinkParentAsync(
        Guid id,
        LinkParentRequest request,
        CancellationToken cancellationToken = default);

    Task<UnlinkParentResponse> UnlinkParentAsync(
        Guid id,
        Guid personId,
        CancellationToken cancellationToken = default);

    Task<SetDeclarationHouseholdResponse> SetDeclarationHouseholdAsync(
        Guid id,
        SetDeclarationHouseholdRequest request,
        CancellationToken cancellationToken = default);

    Task<AttachDocumentResponse> AttachDocumentAsync(
        Guid id,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default);

    Task<BirthDeclarationChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ConfirmBirthDeclarationResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RejectBirthDeclarationResponse> RejectAsync(
        Guid id,
        RejectBirthDeclarationRequest request,
        CancellationToken cancellationToken = default);

    Task<SuspendBirthDeclarationResponse> SuspendAsync(
        Guid id,
        SuspendBirthDeclarationRequest request,
        CancellationToken cancellationToken = default);

    Task<ResumeBirthDeclarationResponse> ResumeAsync(Guid id, CancellationToken cancellationToken = default);
}
