using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Notifications;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class OutboundNotificationRepository(MunicipalDbContext dbContext) : IOutboundNotificationRepository
{
    public async Task AddAsync(OutboundNotification notification, CancellationToken cancellationToken)
    {
        await dbContext.OutboundNotifications.AddAsync(notification, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundNotification>> ListAsync(CancellationToken cancellationToken)
    {
        var notifications = await dbContext.OutboundNotifications
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<OutboundNotification>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var notifications = await dbContext.OutboundNotifications
            .AsNoTracking()
            .Where(n => n.RegistrationCaseId == caseId)
            .ToListAsync(cancellationToken);

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<OutboundNotification>> ClaimPendingBatchAsync(
        int batchSize,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var candidates = await dbContext.OutboundNotifications
            .ToListAsync(cancellationToken);

        var pending = candidates
            .Where(n => n.DeliveryStatus == OutboundNotificationDeliveryStatus.Pending
                && n.NextAttemptAt <= now)
            .OrderBy(n => n.CreatedAt)
            .Take(batchSize)
            .ToList();

        foreach (var notification in pending)
        {
            notification.MarkProcessing();
        }

        if (pending.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return pending;
    }
}
