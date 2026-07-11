namespace SchaerbeekMunicipality.Infrastructure.Integrations;

public sealed class IntegrationOutboxOptions
{
    public const string SectionName = "IntegrationOutbox";

    public bool Enabled { get; set; } = true;

    public int PollIntervalSeconds { get; set; } = 5;

    public int BatchSize { get; set; } = 10;

    public int MaxAttempts { get; set; } = 5;

    public int SimulatedDeliveryDelayMilliseconds { get; set; } = 100;
}
