using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RequestPoliceVerification;

public sealed class RequestPoliceVerificationHandler(
    IRegistrationCaseRepository caseRepository,
    IPoliceVerificationRepository policeVerificationRepository,
    TimeProvider timeProvider)
{
    public async Task<RequestPoliceVerificationResponse> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        var pending = await policeVerificationRepository.GetPendingByCaseIdAsync(caseId, cancellationToken);
        if (pending is not null)
        {
            throw new InvalidPoliceVerificationException(
                "A police verification request is already pending for this case.");
        }

        registrationCase.RequestPoliceVerification();

        var maxAttempt = await policeVerificationRepository.GetMaxAttemptNumberAsync(caseId, cancellationToken);
        var request = PoliceVerificationRequest.Create(
            caseId,
            maxAttempt + 1,
            timeProvider.GetUtcNow());

        await policeVerificationRepository.AddAsync(request, cancellationToken);
        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RequestPoliceVerificationResponse(
            registrationCase.Id.Value,
            request.Id.Value,
            request.AttemptNumber,
            registrationCase.Status.ToString());
    }
}
