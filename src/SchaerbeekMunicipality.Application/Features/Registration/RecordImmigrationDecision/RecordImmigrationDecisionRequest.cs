namespace SchaerbeekMunicipality.Application.Features.Registration.RecordImmigrationDecision;

public sealed record RecordImmigrationDecisionRequest(
    string ReferenceNumber,
    DateOnly DecisionDate);

public sealed record RecordImmigrationDecisionResponse(
    Guid CaseId,
    string ReferenceNumber,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
