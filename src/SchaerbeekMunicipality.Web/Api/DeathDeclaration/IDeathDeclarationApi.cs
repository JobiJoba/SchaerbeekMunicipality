using SchaerbeekMunicipality.Application.Features.DeathDeclaration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ClaimDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ConfirmRadiation;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.GetDeathDeclarationChecklist;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ListDeathDeclarationCases;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RecordDeathFacts;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RejectDeathDeclaration;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.RemoveDocument;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ResumeDeathDeclaration;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.ReviewHousehold;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.SuspendDeathDeclaration;

namespace SchaerbeekMunicipality.Web.Api.DeathDeclaration;

public interface IDeathDeclarationApi
{
    Task<IReadOnlyList<DeathDeclarationCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default);

    Task<OpenDeathDeclarationCaseResponse> OpenCaseAsync(
        OpenDeathDeclarationCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<DeathDeclarationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimDeathDeclarationCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimDeathDeclarationCaseResponse?> TryAutoClaimCaseAsync(Guid id,
        CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RecordDeathFactsResponse> RecordDeathFactsAsync(
        Guid id,
        RecordDeathFactsRequest request,
        CancellationToken cancellationToken = default);

    Task<ReviewHouseholdResponse> ReviewHouseholdAsync(Guid id, CancellationToken cancellationToken = default);

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

    Task<DeathDeclarationChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ConfirmRadiationResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RejectDeathDeclarationResponse> RejectAsync(
        Guid id,
        RejectDeathDeclarationRequest request,
        CancellationToken cancellationToken = default);

    Task<SuspendDeathDeclarationResponse> SuspendAsync(
        Guid id,
        SuspendDeathDeclarationRequest request,
        CancellationToken cancellationToken = default);

    Task<ResumeDeathDeclarationResponse> ResumeAsync(Guid id, CancellationToken cancellationToken = default);
}
