using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class PersonRadiatedNotificationHandler(
    IOutboundNotificationRepository notificationRepository,
    TimeProvider timeProvider) : IPersonRadiatedHandler
{
    public async Task HandleAsync(PersonRadiated domainEvent, CancellationToken cancellationToken)
    {
        var createdAt = timeProvider.GetUtcNow();

        var notifications = new[]
        {
            OutboundNotification.CreateForDeathDeclaration(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.TaxAdministration,
                "Notify tax administration of death and radiation.",
                createdAt),
            OutboundNotification.CreateForDeathDeclaration(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.SocialSecurity,
                "Notify social security of death and radiation.",
                createdAt)
        };

        foreach (var notification in notifications)
            await notificationRepository.AddAsync(notification, cancellationToken);
    }
}
