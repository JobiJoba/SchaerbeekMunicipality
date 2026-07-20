using SchaerbeekMunicipality.Domain.Events;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public interface IPersonRadiatedHandler
{
    Task HandleAsync(PersonRadiated domainEvent, CancellationToken cancellationToken);
}
