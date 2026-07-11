using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.AttachDocument;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ClaimChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ConfirmAddressChange;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.DeclareNewAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.GetChangeOfAddressChecklist;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ListChangeOfAddressCases;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RejectChangeOfAddress;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ReleaseCaseLock;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RemoveDocument;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RequestPoliceVerification;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.UpdateHouseholdForMove;

namespace SchaerbeekMunicipality.Web.Api.ChangeOfAddress;

public sealed class ChangeOfAddressApiClient(HttpClient httpClient)
    : MunicipalApiClientBase(httpClient), IChangeOfAddressApi
{
    private const string BasePath = "/api/change-of-address";

    public Task<IReadOnlyList<ChangeOfAddressCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default) =>
        GetJsonAsync<IReadOnlyList<ChangeOfAddressCaseListItem>>($"{BasePath}/cases", cancellationToken);

    public Task<OpenChangeOfAddressCaseResponse> OpenCaseAsync(
        OpenChangeOfAddressCaseRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<OpenChangeOfAddressCaseRequest, OpenChangeOfAddressCaseResponse>(
            $"{BasePath}/cases",
            request,
            cancellationToken);

    public Task<ChangeOfAddressCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetJsonAsync<ChangeOfAddressCaseDetailDto>($"{BasePath}/cases/{id}", cancellationToken);

    public Task<ClaimChangeOfAddressCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ClaimChangeOfAddressCaseResponse>($"{BasePath}/cases/{id}/claim", cancellationToken);

    public Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ReleaseCaseLockResponse>($"{BasePath}/cases/{id}/release-lock", cancellationToken);

    public Task<DeclareNewAddressResponse> DeclareNewAddressAsync(
        Guid id,
        DeclareNewAddressRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<DeclareNewAddressRequest, DeclareNewAddressResponse>(
            $"{BasePath}/cases/{id}/new-address",
            request,
            cancellationToken);

    public Task<UpdateHouseholdForMoveResponse> UpdateHouseholdForMoveAsync(
        Guid id,
        UpdateHouseholdForMoveRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<UpdateHouseholdForMoveRequest, UpdateHouseholdForMoveResponse>(
            $"{BasePath}/cases/{id}/household",
            request,
            cancellationToken);

    public Task<UnlinkHouseholdMemberResponse> UnlinkHouseholdMemberAsync(
        Guid id,
        Guid personId,
        CancellationToken cancellationToken = default) =>
        DeleteJsonAsync<UnlinkHouseholdMemberResponse>($"{BasePath}/cases/{id}/household/{personId}", cancellationToken);

    public Task<RequestPoliceVerificationResponse> RequestPoliceVerificationAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<RequestPoliceVerificationResponse>($"{BasePath}/cases/{id}/police-verification", cancellationToken);

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

    public Task<ChangeOfAddressChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        GetJsonAsync<ChangeOfAddressChecklistResponse>($"{BasePath}/cases/{id}/checklist", cancellationToken);

    public Task<ConfirmAddressChangeResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default) =>
        PostJsonAsync<ConfirmAddressChangeResponse>($"{BasePath}/cases/{id}/confirm", cancellationToken);

    public Task<RejectChangeOfAddressResponse> RejectAsync(
        Guid id,
        RejectChangeOfAddressRequest request,
        CancellationToken cancellationToken = default) =>
        PostJsonAsync<RejectChangeOfAddressRequest, RejectChangeOfAddressResponse>(
            $"{BasePath}/cases/{id}/reject",
            request,
            cancellationToken);
}
