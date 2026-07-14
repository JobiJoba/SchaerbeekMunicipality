using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Notifications;

public interface IOutboundNotificationRepository
{
    Task AddAsync(OutboundNotification notification, CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboundNotification>> ListAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboundNotification>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboundNotification>> ClaimPendingBatchAsync(
        int batchSize,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}