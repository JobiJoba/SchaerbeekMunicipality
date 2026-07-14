namespace SchaerbeekMunicipality.Domain.Immigration.Policies;

public sealed class ResidencePolicyResult
{
    private ResidencePolicyResult(bool isValid, string? failureReason)
    {
        IsValid = isValid;
        FailureReason = failureReason;
    }

    public bool IsValid { get; }

    public string? FailureReason { get; }

    public static ResidencePolicyResult Valid()
    {
        return new ResidencePolicyResult(true, null);
    }

    public static ResidencePolicyResult Invalid(string reason)
    {
        return new ResidencePolicyResult(false, reason);
    }
}