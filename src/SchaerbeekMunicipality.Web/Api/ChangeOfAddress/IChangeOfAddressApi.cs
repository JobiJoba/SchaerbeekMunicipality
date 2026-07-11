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

public interface IChangeOfAddressApi
{
    Task<IReadOnlyList<ChangeOfAddressCaseListItem>> ListCasesAsync(CancellationToken cancellationToken = default);

    Task<OpenChangeOfAddressCaseResponse> OpenCaseAsync(
        OpenChangeOfAddressCaseRequest request,
        CancellationToken cancellationToken = default);

    Task<ChangeOfAddressCaseDetailDto> GetCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ClaimChangeOfAddressCaseResponse> ClaimCaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ReleaseCaseLockResponse> ReleaseCaseLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DeclareNewAddressResponse> DeclareNewAddressAsync(
        Guid id,
        DeclareNewAddressRequest request,
        CancellationToken cancellationToken = default);

    Task<UpdateHouseholdForMoveResponse> UpdateHouseholdForMoveAsync(
        Guid id,
        UpdateHouseholdForMoveRequest request,
        CancellationToken cancellationToken = default);

    Task<UnlinkHouseholdMemberResponse> UnlinkHouseholdMemberAsync(
        Guid id,
        Guid personId,
        CancellationToken cancellationToken = default);

    Task<RequestPoliceVerificationResponse> RequestPoliceVerificationAsync(
        Guid id,
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

    Task<ChangeOfAddressChecklistResponse> GetChecklistAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ConfirmAddressChangeResponse> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    Task<RejectChangeOfAddressResponse> RejectAsync(
        Guid id,
        RejectChangeOfAddressRequest request,
        CancellationToken cancellationToken = default);
}
