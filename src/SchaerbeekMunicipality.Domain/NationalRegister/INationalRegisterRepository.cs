namespace SchaerbeekMunicipality.Domain.NationalRegister;

public interface INationalRegisterRepository
{
    Task<NationalRegisterPerson?> GetByIdAsync(
        NationalRegisterPersonId id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<NationalRegisterMatch>> SearchAsync(
        NationalRegisterSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
