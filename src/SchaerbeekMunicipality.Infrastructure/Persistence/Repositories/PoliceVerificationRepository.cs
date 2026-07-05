using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class PoliceVerificationRepository(MunicipalDbContext dbContext) : IPoliceVerificationRepository
{
    public Task<PoliceVerificationRequest?> GetByIdAsync(
        PoliceVerificationRequestId id,
        CancellationToken cancellationToken)
    {
        return dbContext.PoliceVerificationRequests
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public Task<PoliceVerificationRequest?> GetPendingByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return dbContext.PoliceVerificationRequests
            .Where(r => r.RegistrationCaseId == caseId && r.CompletedAt == null)
            .OrderByDescending(r => r.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PoliceVerificationRequest>> ListPendingAsync(
        CancellationToken cancellationToken)
    {
        var results = await dbContext.PoliceVerificationRequests
            .AsNoTracking()
            .Where(r => r.CompletedAt == null)
            .ToListAsync(cancellationToken);

        return results
            .OrderBy(r => r.RequestedAt)
            .ToList();
    }

    public async Task<IReadOnlyList<PoliceVerificationRequest>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var results = await dbContext.PoliceVerificationRequests
            .AsNoTracking()
            .Where(r => r.RegistrationCaseId == caseId)
            .ToListAsync(cancellationToken);

        return results
            .OrderByDescending(r => r.AttemptNumber)
            .ToList();
    }

    public Task<int> CountPendingAsync(CancellationToken cancellationToken)
    {
        return dbContext.PoliceVerificationRequests
            .CountAsync(r => r.CompletedAt == null, cancellationToken);
    }

    public async Task<int> GetMaxAttemptNumberAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var hasRequests = await dbContext.PoliceVerificationRequests
            .AnyAsync(r => r.RegistrationCaseId == caseId, cancellationToken);

        if (!hasRequests)
        {
            return 0;
        }

        return await dbContext.PoliceVerificationRequests
            .Where(r => r.RegistrationCaseId == caseId)
            .MaxAsync(r => r.AttemptNumber, cancellationToken);
    }

    public async Task AddAsync(PoliceVerificationRequest request, CancellationToken cancellationToken)
    {
        await dbContext.PoliceVerificationRequests.AddAsync(request, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
