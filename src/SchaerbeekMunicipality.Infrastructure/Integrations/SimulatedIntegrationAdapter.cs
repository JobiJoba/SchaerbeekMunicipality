using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

internal sealed class SimulatedIntegrationAdapter(
    OutboundNotificationRecipient recipient,
    IOptions<IntegrationOutboxOptions> options,
    ILogger<SimulatedIntegrationAdapter> logger) : IIntegrationAdapter
{
    public OutboundNotificationRecipient Recipient => recipient;

    public async Task DeliverAsync(OutboundNotification notification, CancellationToken cancellationToken)
    {
        var delay = options.Value.SimulatedDeliveryDelayMilliseconds;
        if (delay > 0)
        {
            await Task.Delay(delay, cancellationToken);
        }

        logger.LogInformation(
            "Simulated delivery to {Recipient} for notification {NotificationId}: {Message}",
            recipient,
            notification.Id.Value,
            notification.Message);
    }
}
