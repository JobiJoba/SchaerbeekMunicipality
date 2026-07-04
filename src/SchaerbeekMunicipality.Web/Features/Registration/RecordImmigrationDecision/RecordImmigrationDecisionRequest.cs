namespace SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;

public sealed record RecordImmigrationDecisionRequest(
    string ReferenceNumber,
    DateOnly DecisionDate);

public sealed record RecordImmigrationDecisionResponse(
    Guid CaseId,
    string ReferenceNumber,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
