using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
