namespace SchaerbeekMunicipality.Domain.Immigration;

public sealed class ImmigrationDecisionReference
{
    private ImmigrationDecisionReference()
    {
    }

    public string ReferenceNumber { get; private set; } = string.Empty;

    public DateOnly DecisionDate { get; private set; }

    public static ImmigrationDecisionReference Create(ImmigrationDecisionDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(details.ReferenceNumber);

        return new ImmigrationDecisionReference
        {
            ReferenceNumber = details.ReferenceNumber.Trim(),
            DecisionDate = details.DecisionDate,
        };
    }
}
