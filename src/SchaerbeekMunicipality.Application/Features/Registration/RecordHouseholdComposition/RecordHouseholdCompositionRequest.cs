using SchaerbeekMunicipality.Domain.Household;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordHouseholdComposition;

public sealed record HouseholdMemberRequest(
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    HouseholdMemberRole Role);

public sealed record RecordHouseholdCompositionRequest(
    IReadOnlyList<HouseholdMemberRequest> Members);

public sealed record HouseholdMemberResponse(
    Guid Id,
    string GivenName,
    string FamilyName,
    DateOnly BirthDate,
    HouseholdMemberRole Role);

public sealed record RecordHouseholdCompositionResponse(
    Guid CaseId,
    IReadOnlyList<HouseholdMemberResponse> Members);