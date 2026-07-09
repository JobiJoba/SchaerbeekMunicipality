using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ListOutboundNotifications;

public sealed record OutboundNotificationDto(
    Guid Id,
    Guid CaseId,
    Guid PersonId,
    string Recipient,
    string Message,
    DateTimeOffset CreatedAt);

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
            .Select(n => new OutboundNotificationDto(
                n.Id.Value,
                GetSourceCaseId(n),
                n.PersonId.Value,
                n.Recipient.ToString(),
                n.Message,
                n.CreatedAt))
            .ToList();

        return new ListOutboundNotificationsResponse(items.Count, items);
    }

    private static Guid GetSourceCaseId(OutboundNotification notification) =>
        notification.RegistrationCaseId?.Value
        ?? notification.BirthDeclarationCaseId?.Value
        ?? throw new InvalidOperationException("Notification must reference a case.");
}
