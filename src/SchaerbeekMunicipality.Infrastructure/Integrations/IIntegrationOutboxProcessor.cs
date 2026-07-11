namespace SchaerbeekMunicipality.Infrastructure.Integrations;

public interface IIntegrationOutboxProcessor
{
    Task<int> ProcessPendingAsync(CancellationToken cancellationToken);
}
