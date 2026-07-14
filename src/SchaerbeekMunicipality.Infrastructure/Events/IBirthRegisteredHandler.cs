using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public interface IBirthRegisteredHandler
{
    Task HandleAsync(BirthRegistered domainEvent, CancellationToken cancellationToken);
}