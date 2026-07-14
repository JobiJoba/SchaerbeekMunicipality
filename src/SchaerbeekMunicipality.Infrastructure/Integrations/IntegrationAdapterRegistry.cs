using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

internal sealed class IntegrationAdapterRegistry(IEnumerable<IIntegrationAdapter> adapters)
{
    private readonly IReadOnlyDictionary<OutboundNotificationRecipient, IIntegrationAdapter> _adapters =
        adapters.ToDictionary(adapter => adapter.Recipient);

    public IIntegrationAdapter GetRequired(OutboundNotificationRecipient recipient)
    {
        return _adapters.TryGetValue(recipient, out var adapter)
            ? adapter
            : throw new InvalidOperationException($"No integration adapter registered for '{recipient}'.");
    }
}