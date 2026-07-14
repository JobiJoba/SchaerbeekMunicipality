namespace SchaerbeekMunicipality.Domain.BirthDeclaration;

public interface IBirthDeclarationCaseRepository
{
    Task<IReadOnlyList<BirthDeclarationCase>> ListAsync(CancellationToken cancellationToken);

    Task<BirthDeclarationCase?> GetByIdAsync(BirthDeclarationCaseId id, CancellationToken cancellationToken);

    Task AddAsync(BirthDeclarationCase birthDeclarationCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}