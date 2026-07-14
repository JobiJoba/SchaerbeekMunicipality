using Microsoft.EntityFrameworkCore;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Infrastructure.Persistence.Repositories;

internal sealed class ResidencePermitRepository(MunicipalDbContext dbContext) : IResidencePermitRepository
{
    public Task<ResidencePermit?> GetByCaseIdAsync(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        return dbContext.ResidencePermits
            .FirstOrDefaultAsync(p => p.RegistrationCaseId == caseId, cancellationToken);
    }

    public async Task AddAsync(ResidencePermit permit, CancellationToken cancellationToken)
    {
        await dbContext.ResidencePermits.AddAsync(permit, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}