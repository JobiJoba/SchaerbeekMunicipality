using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class BirthRegisteredNotificationHandler(
    IOutboundNotificationRepository notificationRepository,
    TimeProvider timeProvider) : IBirthRegisteredHandler
{
    public async Task HandleAsync(BirthRegistered domainEvent, CancellationToken cancellationToken)
    {
        var createdAt = timeProvider.GetUtcNow();

        var notifications = new[]
        {
            OutboundNotification.CreateForBirthDeclaration(
                domainEvent.CaseId,
                domainEvent.ChildPersonId,
                OutboundNotificationRecipient.TaxAdministration,
                "Notify tax administration of birth registration.",
                createdAt),
            OutboundNotification.CreateForBirthDeclaration(
                domainEvent.CaseId,
                domainEvent.ChildPersonId,
                OutboundNotificationRecipient.HealthInsurance,
                "Notify health insurance mutuality of birth.",
                createdAt)
        };

        foreach (var notification in notifications)
            await notificationRepository.AddAsync(notification, cancellationToken);
    }
}