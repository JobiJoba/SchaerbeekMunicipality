using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.ListOutboundNotifications;

public enum OutboundNotificationCaseType
{
    Registration,
    BirthDeclaration,
    ChangeOfAddress,
    DeathDeclaration
}

public sealed record OutboundNotificationDto(
    Guid Id,
    Guid CaseId,
    OutboundNotificationCaseType CaseType,
    Guid PersonId,
    string Recipient,
    string Message,
    DateTimeOffset CreatedAt,
    string DeliveryStatus,
    int AttemptCount,
    DateTimeOffset? SentAt,
    string? LastError);

public sealed record ListOutboundNotificationsResponse(
    int TotalCount,
    IReadOnlyList<OutboundNotificationDto> Items);

public sealed class ListOutboundNotificationsHandler(IOutboundNotificationRepository notificationRepository)
{
    public async Task<ListOutboundNotificationsResponse> Handle(
        RegistrationCaseId? caseId,
        CancellationToken cancellationToken)
    {
        var notifications = caseId is { } filterCaseId
            ? await notificationRepository.ListByCaseIdAsync(filterCaseId, cancellationToken)
            : await notificationRepository.ListAsync(cancellationToken);

        var items = notifications
            .Select(n =>
            {
                var (sourceCaseId, caseType) = MapSourceCase(n);
                return new OutboundNotificationDto(
                    n.Id.Value,
                    sourceCaseId,
                    caseType,
                    n.PersonId.Value,
                    n.Recipient.ToString(),
                    n.Message,
                    n.CreatedAt,
                    n.DeliveryStatus.ToString(),
                    n.AttemptCount,
                    n.SentAt,
                    n.LastError);
            })
            .ToList();

        return new ListOutboundNotificationsResponse(items.Count, items);
    }

    private static (Guid CaseId, OutboundNotificationCaseType CaseType) MapSourceCase(
        OutboundNotification notification)
    {
        if (notification.RegistrationCaseId is { } registrationCaseId)
            return (registrationCaseId.Value, OutboundNotificationCaseType.Registration);

        if (notification.BirthDeclarationCaseId is { } birthDeclarationCaseId)
            return (birthDeclarationCaseId.Value, OutboundNotificationCaseType.BirthDeclaration);

        if (notification.ChangeOfAddressCaseId is { } changeOfAddressCaseId)
            return (changeOfAddressCaseId.Value, OutboundNotificationCaseType.ChangeOfAddress);

        if (notification.DeathDeclarationCaseId is { } deathDeclarationCaseId)
            return (deathDeclarationCaseId.Value, OutboundNotificationCaseType.DeathDeclaration);

        throw new InvalidOperationException("Notification must reference a case.");
    }
}