using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

internal sealed class IntegrationOutboxProcessor(
    IOutboundNotificationRepository notificationRepository,
    IntegrationAdapterRegistry adapterRegistry,
    IOptions<IntegrationOutboxOptions> options,
    TimeProvider timeProvider,
    ILogger<IntegrationOutboxProcessor> logger) : IIntegrationOutboxProcessor
{
    public async Task<int> ProcessPendingAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var now = timeProvider.GetUtcNow();
        var claimed = await notificationRepository.ClaimPendingBatchAsync(
            settings.BatchSize,
            now,
            cancellationToken);

        if (claimed.Count == 0)
        {
            return 0;
        }

        foreach (var notification in claimed)
        {
            try
            {
                var adapter = adapterRegistry.GetRequired(notification.Recipient);
                await adapter.DeliverAsync(notification, cancellationToken);
                notification.MarkSent(timeProvider.GetUtcNow());
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogWarning(
                    ex,
                    "Failed to deliver notification {NotificationId} to {Recipient} (attempt {AttemptCount})",
                    notification.Id.Value,
                    notification.Recipient,
                    notification.AttemptCount + 1);

                notification.RecordDeliveryFailure(ex.Message, timeProvider.GetUtcNow(), settings.MaxAttempts);
            }
        }

        await notificationRepository.SaveChangesAsync(cancellationToken);
        return claimed.Count;
    }
}
