using SchaerbeekMunicipality.Domain.Address;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordHousingSituation;

public sealed record RecordHousingSituationRequest(HousingSituation Situation);

public sealed record RecordHousingSituationResponse(
    Guid CaseId,
    HousingSituation Situation);
