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

public sealed class BirthDeclarationApiClient(HttpClient httpClient)
    : MunicipalApiClientBase(httpClient), IBirthDeclarationApi
{
    private const string BasePath = "/api/birth-declarations";

    public Task<IReadOnlyList<BirthDeclarationCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default) =>
        GetJsonAsync<IReadOnlyList<BirthDeclarationCaseListItem>>($"{BasePath}/cases", cancellationToken);

    public Task<OpenBirthDeclarationCaseResponse> OpenCaseAsync(CancellationToken cancellationToken = default) =>
        PostJsonAsync<OpenBirthDeclarationCaseResponse>($"{BasePath}/cases", cancellationToken);

    public Task<BirthDeclarationCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetJsonAsync<BirthDeclarationCaseDetailDto>($"{BasePath}/cases/{id}", cancellationToken);

    public Task<ClaimBirthDeclarationCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ClaimBirthDeclarationCaseResponse>($"{BasePath}/cases/{id}/claim", cancellationToken);

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/cases/{id}/release-lock", cancellationToken);

    public Task<RecordChildDetailsResponse> RecordChildDetailsAsync(
        Guid id,
        RecordChildDetailsRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<RecordChildDetailsRequest, RecordChildDetailsResponse>(
            $"{BasePath}/cases/{id}/child-details",
            request,
            cancellationToken);

    public Task<LinkParentResponse> LinkParentAsync(
        Guid id,
        LinkParentRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<LinkParentRequest, LinkParentResponse>(
            $"{BasePath}/cases/{id}/parents/link",
            request,
            cancellationToken);

    public Task<UnlinkParentResponse> UnlinkParentAsync(
        Guid id,
        Guid personId,
        CancellationToken cancellationToken = default) =>
        DeleteJsonAsync<UnlinkParentResponse>($"{BasePath}/cases/{id}/parents/{personId}", cancellationToken);

    public Task<SetDeclarationHouseholdResponse> SetDeclarationHouseholdAsync(
        Guid id,
        SetDeclarationHouseholdRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<SetDeclarationHouseholdRequest, SetDeclarationHouseholdResponse>(
            $"{BasePath}/cases/{id}/household",
            request,
            cancellationToken);

    public Task<AttachDocumentResponse> AttachDocumentAsync(
        Guid id,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default) =>
        PostMultipartFileAsync<AttachDocumentResponse>(
            $"{BasePath}/cases/{id}/documents",
            fileStream,
            fileName,
            cancellationToken);

    public Task<Stream> DownloadDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default) =>
        DownloadStreamAsync($"{BasePath}/cases/{id}/documents/{documentId}", cancellationToken);

    public Task<RemoveDocumentResponse> RemoveDocumentAsync(
        Guid id,
        Guid documentId,
        CancellationToken cancellationToken = default) =>
        DeleteJsonAsync<RemoveDocumentResponse>($"{BasePath}/cases/{id}/documents/{documentId}", cancellationToken);

    public Task<BirthDeclarationChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        GetJsonAsync<BirthDeclarationChecklistResponse>($"{BasePath}/cases/{id}/checklist", cancellationToken);

    public Task<ConfirmBirthDeclarationResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ConfirmBirthDeclarationResponse>($"{BasePath}/cases/{id}/confirm", cancellationToken);

    public Task<RejectBirthDeclarationResponse> RejectAsync(
        Guid id,
        RejectBirthDeclarationRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<RejectBirthDeclarationRequest, RejectBirthDeclarationResponse>(
            $"{BasePath}/cases/{id}/reject",
            request,
            cancellationToken);

    public Task<SuspendBirthDeclarationResponse> SuspendAsync(
        Guid id,
        SuspendBirthDeclarationRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<SuspendBirthDeclarationRequest, SuspendBirthDeclarationResponse>(
            $"{BasePath}/cases/{id}/suspend",
            request,
            cancellationToken);

    public Task<ResumeBirthDeclarationResponse> ResumeAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ResumeBirthDeclarationResponse>($"{BasePath}/cases/{id}/resume", cancellationToken);
}
