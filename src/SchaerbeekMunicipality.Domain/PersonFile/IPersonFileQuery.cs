using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.PersonFile;

public sealed record PersonCaseSummary(
    Guid CaseId,
    string Workflow,
    string Status,
    DateTimeOffset OpenedAt,
    DateTimeOffset? ClosedAt,
    string DetailPath);

public sealed record PersonHouseholdMemberSummary(
    Guid? PersonId,
    string GivenName,
    string FamilyName,
    DateOnly? BirthDate,
    string Role,
    string Source);

public sealed record PersonAddressHistoryEntry(
    string Street,
    string HouseNumber,
    string? Box,
    string PostalCode,
    string Municipality,
    string? HousingSituation,
    DateTimeOffset? EffectiveFrom,
    bool IsCurrent,
    string Source);

public sealed record PersonHistoryEvent(
    string Title,
    DateTimeOffset Timestamp,
    string? Description,
    string Source);

public interface IPersonFileQuery
{
    Task<RegisterTarget?> GetRegisterTargetAsync(PersonId personId, CancellationToken cancellationToken);

    Task<IReadOnlyList<PersonCaseSummary>> ListCasesByPersonIdAsync(
        PersonId personId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PersonHouseholdMemberSummary>> GetHouseholdMembersAsync(
        PersonId personId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PersonAddressHistoryEntry>> ListAddressHistoryAsync(
        PersonId personId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PersonHistoryEvent>> ListHistoryEventsAsync(
        PersonId personId,
        CancellationToken cancellationToken);
}