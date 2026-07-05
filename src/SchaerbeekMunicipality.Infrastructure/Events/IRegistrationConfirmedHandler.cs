using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public interface IRegistrationConfirmedHandler
{
    Task HandleAsync(RegistrationConfirmed domainEvent, CancellationToken cancellationToken);
}
