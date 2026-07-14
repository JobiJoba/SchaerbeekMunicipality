using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Integrations;

public interface IIntegrationAdapter
{
    OutboundNotificationRecipient Recipient { get; }

    Task DeliverAsync(OutboundNotification notification, CancellationToken cancellationToken);
}