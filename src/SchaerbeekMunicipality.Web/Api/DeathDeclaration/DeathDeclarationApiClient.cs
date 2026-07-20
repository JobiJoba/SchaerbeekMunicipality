using SchaerbeekMunicipality.Application.Auth;
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

public sealed class DeathDeclarationApiClient(HttpClient httpClient, ICurrentOfficer currentOfficer)
    : MunicipalApiClientBase(httpClient, currentOfficer), IDeathDeclarationApi
{
    private const string BasePath = "/api/death-declarations";

    public Task<IReadOnlyList<DeathDeclarationCaseListItem>> ListCasesAsync(
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<IReadOnlyList<DeathDeclarationCaseListItem>>($"{BasePath}/cases", cancellationToken);
    }

    public Task<OpenDeathDeclarationCaseResponse> OpenCaseAsync(
        OpenDeathDeclarationCaseRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<OpenDeathDeclarationCaseRequest, OpenDeathDeclarationCaseResponse>(
            $"{BasePath}/cases",
            request,
            cancellationToken);
    }

    public Task<DeathDeclarationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<DeathDeclarationCaseDetailDto>($"{BasePath}/cases/{id}", cancellationToken);
    }

    public Task<ClaimDeathDeclarationCaseResponse> ClaimCaseAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ClaimDeathDeclarationCaseResponse>($"{BasePath}/cases/{id}/claim", cancellationToken);
    }

    public Task<ClaimDeathDeclarationCaseResponse?> TryAutoClaimCaseAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return PostJsonOptionalAsync<ClaimDeathDeclarationCaseResponse>($"{BasePath}/cases/{id}/auto-claim",
            cancellationToken);
    }

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/cases/{id}/release-lock", cancellationToken);
    }

    public Task<RecordDeathFactsResponse> RecordDeathFactsAsync(
        Guid id,
        RecordDeathFactsRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RecordDeathFactsRequest, RecordDeathFactsResponse>(
            $"{BasePath}/cases/{id}/death-facts",
            request,
            cancellationToken);
    }

    public Task<ReviewHouseholdResponse> ReviewHouseholdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ReviewHouseholdResponse>($"{BasePath}/cases/{id}/review-household", cancellationToken);
    }

    public Task<AttachDocumentResponse> AttachDocumentAsync(
        Guid id,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        return PostMultipartFileAsync<AttachDocumentResponse>(
            $"{BasePath}/cases/{id}/documents",
            fileStream,
            fileName,
            cancellationToken);
    }

    public Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DownloadStreamAsync($"{BasePath}/cases/{id}/documents/{documentId}", cancellationToken);
    }

    public Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        return DeleteJsonAsync<RemoveDocumentResponse>($"{BasePath}/cases/{id}/documents/{documentId}",
            cancellationToken);
    }

    public Task<DeathDeclarationChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return GetJsonAsync<DeathDeclarationChecklistResponse>($"{BasePath}/cases/{id}/checklist", cancellationToken);
    }

    public Task<ConfirmRadiationResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ConfirmRadiationResponse>($"{BasePath}/cases/{id}/confirm", cancellationToken);
    }

    public Task<RejectDeathDeclarationResponse> RejectAsync(
        Guid id,
        RejectDeathDeclarationRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<RejectDeathDeclarationRequest, RejectDeathDeclarationResponse>(
            $"{BasePath}/cases/{id}/reject",
            request,
            cancellationToken);
    }

    public Task<SuspendDeathDeclarationResponse> SuspendAsync(
        Guid id,
        SuspendDeathDeclarationRequest request,
        CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<SuspendDeathDeclarationRequest, SuspendDeathDeclarationResponse>(
            $"{BasePath}/cases/{id}/suspend",
            request,
            cancellationToken);
    }

    public Task<ResumeDeathDeclarationResponse> ResumeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return PostJsonAsync<ResumeDeathDeclarationResponse>($"{BasePath}/cases/{id}/resume", cancellationToken);
    }
}
