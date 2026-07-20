namespace SchaerbeekMunicipality.Domain.DeathDeclaration;

public sealed record DeathFacts(
    DateOnly DeathDate,
    string DeathPlace,
    bool DeathAbroad,
    InformantRelationship InformantRelationship);
