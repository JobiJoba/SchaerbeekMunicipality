using Microsoft.Extensions.Logging;
using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class DomainEventDispatcher(
    IEnumerable<IRegistrationConfirmedHandler> registrationConfirmedHandlers,
    IEnumerable<IBirthRegisteredHandler> birthRegisteredHandlers,
    IEnumerable<IAddressChangedHandler> addressChangedHandlers,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        switch (domainEvent)
        {
            case RegistrationConfirmed confirmed:
                foreach (var handler in registrationConfirmedHandlers)
                    await handler.HandleAsync(confirmed, cancellationToken);

                break;
            case BirthRegistered birthRegistered:
                foreach (var handler in birthRegisteredHandlers)
                    await handler.HandleAsync(birthRegistered, cancellationToken);

                break;
            case AddressChanged addressChanged:
                foreach (var handler in addressChangedHandlers)
                    await handler.HandleAsync(addressChanged, cancellationToken);

                break;
            default:
                logger.LogDebug("No handler registered for domain event {EventType}", domainEvent.GetType().Name);
                break;
        }
    }
}