using SchaerbeekMunicipality.Domain.Events;
using SchaerbeekMunicipality.Domain.Notifications;

namespace SchaerbeekMunicipality.Infrastructure.Events;

public sealed class RegistrationConfirmedNotificationHandler(
    IOutboundNotificationRepository notificationRepository,
    TimeProvider timeProvider) : IRegistrationConfirmedHandler
{
    public async Task HandleAsync(RegistrationConfirmed domainEvent, CancellationToken cancellationToken)
    {
        var createdAt = timeProvider.GetUtcNow();

        var notifications = new[]
        {
            OutboundNotification.Create(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.TaxAdministration,
                "Notify tax administration of new registration.",
                createdAt),
            OutboundNotification.Create(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.SocialSecurity,
                "Notify social security (ONSS/RSZ) of registration.",
                createdAt),
            OutboundNotification.Create(
                domainEvent.CaseId,
                domainEvent.PersonId,
                OutboundNotificationRecipient.HealthInsurance,
                "Notify health insurance mutuality of registration.",
                createdAt)
        };

        foreach (var notification in notifications)
            await notificationRepository.AddAsync(notification, cancellationToken);
    }
}