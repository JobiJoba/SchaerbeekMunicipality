using FluentValidation;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.RejectChangeOfAddress;

public sealed class RejectChangeOfAddressHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectChangeOfAddressRequest> validator)
{
    public async Task<RejectChangeOfAddressResponse> Handle(
        ChangeOfAddressCaseId caseId,
        RejectChangeOfAddressRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can reject change of address cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RejectChangeOfAddress),
            cancellationToken);

        changeOfAddressCase.Reject(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectChangeOfAddressResponse(
            changeOfAddressCase.Id.Value,
            changeOfAddressCase.Status.ToString(),
            request.Reason.ToString());
    }
}
