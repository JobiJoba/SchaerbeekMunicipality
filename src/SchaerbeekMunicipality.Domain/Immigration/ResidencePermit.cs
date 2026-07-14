using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Domain.Immigration;

public sealed class ResidencePermit
{
    private ResidencePermit()
    {
    }

    public ResidencePermitId Id { get; private set; }

    public RegistrationCaseId RegistrationCaseId { get; private set; }

    public ResidencePermitType PermitType { get; private set; }

    public string? CardNumber { get; private set; }

    public DateOnly ValidFrom { get; private set; }

    public DateOnly ValidUntil { get; private set; }

    public string? IssuingAuthority { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    public static ResidencePermit Create(
        RegistrationCaseId registrationCaseId,
        ResidencePermitDetails details,
        DateTimeOffset recordedAt)
    {
        if (details.ValidUntil < details.ValidFrom)
            throw new ArgumentException("Permit valid-until date must be on or after valid-from date.");

        return new ResidencePermit
        {
            Id = ResidencePermitId.New(),
            RegistrationCaseId = registrationCaseId,
            PermitType = details.PermitType,
            CardNumber = string.IsNullOrWhiteSpace(details.CardNumber) ? null : details.CardNumber.Trim(),
            ValidFrom = details.ValidFrom,
            ValidUntil = details.ValidUntil,
            IssuingAuthority = string.IsNullOrWhiteSpace(details.IssuingAuthority)
                ? null
                : details.IssuingAuthority.Trim(),
            RecordedAt = recordedAt
        };
    }

    public void Update(ResidencePermitDetails details, DateTimeOffset recordedAt)
    {
        if (details.ValidUntil < details.ValidFrom)
            throw new ArgumentException("Permit valid-until date must be on or after valid-from date.");

        PermitType = details.PermitType;
        CardNumber = string.IsNullOrWhiteSpace(details.CardNumber) ? null : details.CardNumber.Trim();
        ValidFrom = details.ValidFrom;
        ValidUntil = details.ValidUntil;
        IssuingAuthority = string.IsNullOrWhiteSpace(details.IssuingAuthority)
            ? null
            : details.IssuingAuthority.Trim();
        RecordedAt = recordedAt;
    }
}