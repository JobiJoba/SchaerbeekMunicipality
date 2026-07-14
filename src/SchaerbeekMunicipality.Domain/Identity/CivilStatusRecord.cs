using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Identity;

public sealed class CivilStatusRecord
{
    private CivilStatusRecord()
    {
    }

    public CivilStatus Status { get; private set; }

    public string? SpouseGivenName { get; private set; }

    public string? SpouseFamilyName { get; private set; }

    public DateOnly? MarriageDate { get; private set; }

    public string? MarriagePlace { get; private set; }

    public MarriageRecognitionStatus MarriageRecognitionStatus { get; private set; }

    public CivilStatus EffectiveRegisterStatus =>
        IsMarriageRecognitionBlocking()
            ? CivilStatus.Single
            : Status;

    public static CivilStatusRecord Create(CivilStatusDetails details)
    {
        if (RequiresMarriageDetails(details.Status))
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(details.SpouseGivenName);
            ArgumentException.ThrowIfNullOrWhiteSpace(details.SpouseFamilyName);

            if (details.MarriageDate is null)
                throw new ArgumentException("Marriage date is required for this civil status.");
        }

        return new CivilStatusRecord
        {
            Status = details.Status,
            SpouseGivenName = string.IsNullOrWhiteSpace(details.SpouseGivenName)
                ? null
                : details.SpouseGivenName.Trim(),
            SpouseFamilyName = string.IsNullOrWhiteSpace(details.SpouseFamilyName)
                ? null
                : details.SpouseFamilyName.Trim(),
            MarriageDate = details.MarriageDate,
            MarriagePlace = string.IsNullOrWhiteSpace(details.MarriagePlace)
                ? null
                : details.MarriagePlace.Trim(),
            MarriageRecognitionStatus = ResolveMarriageRecognitionStatus(details)
        };
    }

    public bool IsMarriageRecognitionBlocking()
    {
        return RegistrationExceptionRules.IsMarriageRecognitionBlocking(this);
    }

    private static MarriageRecognitionStatus ResolveMarriageRecognitionStatus(CivilStatusDetails details)
    {
        if (!RequiresMarriageDetails(details.Status)) return MarriageRecognitionStatus.NotApplicable;

        if (details.MarriageRecognitionStatus != MarriageRecognitionStatus.NotApplicable)
            return details.MarriageRecognitionStatus;

        if (string.IsNullOrWhiteSpace(details.MarriagePlace)) return MarriageRecognitionStatus.NotApplicable;

        var record = new CivilStatusRecord
        {
            Status = details.Status,
            MarriagePlace = details.MarriagePlace.Trim(),
            MarriageRecognitionStatus = MarriageRecognitionStatus.NotApplicable
        };

        return RegistrationExceptionRules.IsMarriageAbroad(record)
            ? MarriageRecognitionStatus.PendingRecognition
            : MarriageRecognitionStatus.Recognised;
    }

    public static bool RequiresMarriageDetails(CivilStatus status)
    {
        return status is CivilStatus.Married or CivilStatus.RegisteredPartnership;
    }
}