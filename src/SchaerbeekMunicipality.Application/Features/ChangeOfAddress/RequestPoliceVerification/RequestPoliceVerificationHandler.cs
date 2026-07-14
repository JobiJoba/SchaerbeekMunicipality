using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Police;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RequestPoliceVerification;

public sealed class RequestPoliceVerificationHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IPoliceVerificationRepository policeVerificationRepository,
    TimeProvider timeProvider)
{
    public async Task<RequestPoliceVerificationResponse> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RequestPoliceVerification),
            cancellationToken);

        var pending = await policeVerificationRepository.GetPendingByChangeOfAddressCaseIdAsync(
            caseId,
            cancellationToken);
        if (pending is not null)
            throw new InvalidPoliceVerificationException(
                "A police verification request is already pending for this case.");

        changeOfAddressCase.RequestPoliceVerification();

        var maxAttempt = await policeVerificationRepository.GetMaxAttemptNumberForChangeOfAddressAsync(
            caseId,
            cancellationToken);
        var request = PoliceVerificationRequest.CreateForChangeOfAddressCase(
            caseId,
            maxAttempt + 1,
            timeProvider.GetUtcNow());

        await policeVerificationRepository.AddAsync(request, cancellationToken);
        changeOfAddressCase.LinkPoliceVerificationRequest(request.Id);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RequestPoliceVerificationResponse(
            changeOfAddressCase.Id.Value,
            request.Id.Value,
            request.AttemptNumber,
            changeOfAddressCase.Status.ToString());
    }
}