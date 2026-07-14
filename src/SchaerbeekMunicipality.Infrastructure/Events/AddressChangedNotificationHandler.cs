using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public interface IAddressChangedHandler
{
    Task HandleAsync(AddressChanged domainEvent, CancellationToken cancellationToken);
}

public sealed class AddressChangedNotificationHandler(
    IOutboundNotificationRepository notificationRepository,
    TimeProvider timeProvider) : IAddressChangedHandler
{
    public async Task HandleAsync(AddressChanged domainEvent, CancellationToken cancellationToken)
    {
        var createdAt = timeProvider.GetUtcNow();

        var notifications = new[]
        {
            OutboundNotification.CreateForChangeOfAddress(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.TaxAdministration,
                "Notify tax administration of domicile change.",
                createdAt),
            OutboundNotification.CreateForChangeOfAddress(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.SocialSecurity,
                "Notify electoral roll of domicile change.",
                createdAt)
        };

        foreach (var notification in notifications)
            await notificationRepository.AddAsync(notification, cancellationToken);
    }
}