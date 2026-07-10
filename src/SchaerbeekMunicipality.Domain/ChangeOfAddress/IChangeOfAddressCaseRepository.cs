namespace SchaerbeekMunicipality.Domain.ChangeOfAddress;

public interface IChangeOfAddressCaseRepository
{
    Task<IReadOnlyList<ChangeOfAddressCase>> ListAsync(CancellationToken cancellationToken);

    Task<ChangeOfAddressCase?> GetByIdAsync(ChangeOfAddressCaseId id, CancellationToken cancellationToken);

    Task AddAsync(ChangeOfAddressCase changeOfAddressCase, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
