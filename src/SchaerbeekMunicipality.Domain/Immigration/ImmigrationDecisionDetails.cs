namespace SchaerbeekMunicipality.Domain.Immigration;

public sealed record ImmigrationDecisionDetails(
    string ReferenceNumber,
    DateOnly DecisionDate);