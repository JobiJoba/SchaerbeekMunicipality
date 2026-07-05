using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Certificates;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class CertificateRequestRepository(MunicipalDbContext dbContext) : ICertificateRequestRepository
{
    public async Task AddAsync(CertificateRequest certificateRequest, CancellationToken cancellationToken)
    {
        await dbContext.CertificateRequests.AddAsync(certificateRequest, cancellationToken);
    }

    public async Task<IReadOnlyList<CertificateRequest>> ListByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var certificates = await dbContext.CertificateRequests
            .AsNoTracking()
            .Where(c => c.RegistrationCaseId == caseId)
            .ToListAsync(cancellationToken);

        return certificates
            .OrderByDescending(c => c.IssuedAt)
            .ToList();
    }

    public Task<int> CountByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken)
    {
        return dbContext.CertificateRequests
            .AsNoTracking()
            .CountAsync(c => c.CertificateType == certificateType, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
