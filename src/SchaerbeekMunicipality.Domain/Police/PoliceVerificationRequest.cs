using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Police;

public sealed class PoliceVerificationRequest
{
    private PoliceVerificationRequest()
    {
    }

    public PoliceVerificationRequestId Id { get; private set; }

    public RegistrationCaseId? RegistrationCaseId { get; private set; }

    public ChangeOfAddressCaseId? ChangeOfAddressCaseId { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public PoliceVerificationResult? Result { get; private set; }

    public string? OfficerNotes { get; private set; }

    public int AttemptNumber { get; private set; }

    public bool IsPending => CompletedAt is null;

    public static PoliceVerificationRequest CreateForRegistrationCase(
        RegistrationCaseId registrationCaseId,
        int attemptNumber,
        DateTimeOffset requestedAt)
    {
        if (attemptNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber), "Attempt number must be at least 1.");
        }

        return new PoliceVerificationRequest
        {
            Id = PoliceVerificationRequestId.New(),
            RegistrationCaseId = registrationCaseId,
            AttemptNumber = attemptNumber,
            RequestedAt = requestedAt,
        };
    }

    public static PoliceVerificationRequest CreateForChangeOfAddressCase(
        ChangeOfAddressCaseId changeOfAddressCaseId,
        int attemptNumber,
        DateTimeOffset requestedAt)
    {
        if (attemptNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(attemptNumber), "Attempt number must be at least 1.");
        }

        return new PoliceVerificationRequest
        {
            Id = PoliceVerificationRequestId.New(),
            ChangeOfAddressCaseId = changeOfAddressCaseId,
            AttemptNumber = attemptNumber,
            RequestedAt = requestedAt,
        };
    }

    public static PoliceVerificationRequest Create(
        RegistrationCaseId registrationCaseId,
        int attemptNumber,
        DateTimeOffset requestedAt) =>
        CreateForRegistrationCase(registrationCaseId, attemptNumber, requestedAt);

    public void RecordResult(
        PoliceVerificationResult result,
        string? officerNotes,
        DateTimeOffset completedAt)
    {
        if (!IsPending)
        {
            throw new InvalidPoliceVerificationException(
                "This police verification request has already been completed.");
        }

        Result = result;
        OfficerNotes = string.IsNullOrWhiteSpace(officerNotes) ? null : officerNotes.Trim();
        CompletedAt = completedAt;
    }
}
