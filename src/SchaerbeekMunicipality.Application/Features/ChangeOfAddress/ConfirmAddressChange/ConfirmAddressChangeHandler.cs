using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Infrastructure.Events;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.ConfirmAddressChange;

public sealed class ConfirmAddressChangeHandler(
    ChangeOfAddressCaseGuard caseGuard,
    IChangeOfAddressCaseRepository caseRepository,
    IPersonRepository personRepository,
    IDomainEventDispatcher domainEventDispatcher,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider)
{
    public async Task<ConfirmAddressChangeResponse> Handle(
        ChangeOfAddressCaseId caseId,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can confirm address changes.");

        var changeOfAddressCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ConfirmAddressChange),
            cancellationToken);

        if (changeOfAddressCase.NewAddress is null)
            throw new InvalidChangeOfAddressTransitionException(
                "A new address must be declared before confirmation.");

        var person = await personRepository.GetForUpdateAsync(changeOfAddressCase.PersonId, cancellationToken)
                     ?? throw new KeyNotFoundException($"Person '{changeOfAddressCase.PersonId}' was not found.");

        person.UpdateDomicile(changeOfAddressCase.NewAddress);

        var confirmedAt = timeProvider.GetUtcNow();
        var eventDetails = changeOfAddressCase.ConfirmAddressChange(confirmedAt);

        await domainEventDispatcher.DispatchAsync(
            new AddressChanged(
                eventDetails.CaseId,
                eventDetails.PersonId,
                eventDetails.ConfirmedAt),
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ConfirmAddressChangeResponse(
            changeOfAddressCase.Id.Value,
            changeOfAddressCase.Status.ToString(),
            person.Id.Value);
    }
}