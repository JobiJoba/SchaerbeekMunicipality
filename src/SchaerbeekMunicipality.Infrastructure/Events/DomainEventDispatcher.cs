using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class DomainEventDispatcher(
    IServiceProvider serviceProvider,
    ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        switch (domainEvent)
        {
            case RegistrationConfirmed confirmed:
                await DispatchRegistrationConfirmedAsync(confirmed, cancellationToken);
                break;
            case BirthRegistered birthRegistered:
                await DispatchBirthRegisteredAsync(birthRegistered, cancellationToken);
                break;
            default:
                logger.LogDebug("No handler registered for domain event {EventType}", domainEvent.GetType().Name);
                break;
        }
    }

    private async Task DispatchRegistrationConfirmedAsync(
        RegistrationConfirmed confirmed,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IRegistrationConfirmedHandler>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(confirmed, cancellationToken);
        }
    }

    private async Task DispatchBirthRegisteredAsync(
        BirthRegistered birthRegistered,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var handlers = scope.ServiceProvider.GetServices<IBirthRegisteredHandler>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(birthRegistered, cancellationToken);
        }
    }
}
