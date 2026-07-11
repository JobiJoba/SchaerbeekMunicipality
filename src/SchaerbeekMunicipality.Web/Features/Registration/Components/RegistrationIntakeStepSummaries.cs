using SchaerbeekMunicipality.Domain.Address;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Data;
using SchaerbeekMunicipality.Application.Features.Registration.GetRegistrationCase;

namespace SchaerbeekMunicipality.Web.Features.Registration.Components;

public static class RegistrationIntakeStepSummaries
{
    private static readonly RegistrationIntakeStep[] StepOrder =
    [
        RegistrationIntakeStep.Identity,
        RegistrationIntakeStep.LegalResidence,
        RegistrationIntakeStep.Address,
        RegistrationIntakeStep.Household,
        RegistrationIntakeStep.CivilStatus,
        RegistrationIntakeStep.BirthInformation,
        RegistrationIntakeStep.PoliceVerification,
    ];

    public static bool IsVisible(RegistrationIntakeStep step, RegistrationCaseDetailDto dto) =>
        step != RegistrationIntakeStep.PoliceVerification ||
        dto.ActivePoliceVerification is not null ||
        dto.PoliceVerificationHistory.Count > 0;

    public static bool IsComplete(RegistrationIntakeStep step, RegistrationCaseDetailDto dto) => step switch
    {
        RegistrationIntakeStep.Identity => dto.Checklist.IdentityEstablished,
        RegistrationIntakeStep.LegalResidence => dto.Checklist.LegalResidenceEstablished,
        RegistrationIntakeStep.Address => dto.Checklist.AddressDeclared,
        RegistrationIntakeStep.Household => dto.HouseholdMembers.Count > 0,
        RegistrationIntakeStep.CivilStatus => dto.CivilStatus is not null,
        RegistrationIntakeStep.BirthInformation => dto.Person?.BirthInformation is not null,
        RegistrationIntakeStep.PoliceVerification => dto.ActivePoliceVerification is null,
        _ => false,
    };

    public static IReadOnlySet<RegistrationIntakeStep> GetDefaultExpandedSteps(RegistrationCaseDetailDto dto)
    {
        var expanded = new HashSet<RegistrationIntakeStep> { RegistrationIntakeStep.Identity };

        if (!IsComplete(RegistrationIntakeStep.Identity, dto))
        {
            return expanded;
        }

        foreach (var step in StepOrder)
        {
            if (step == RegistrationIntakeStep.Identity)
            {
                continue;
            }

            if (!IsVisible(step, dto))
            {
                continue;
            }

            if (!IsComplete(step, dto))
            {
                expanded.Add(step);
                break;
            }
        }

        return expanded;
    }

    public static string GetSummary(RegistrationIntakeStep step, RegistrationCaseDetailDto dto) => step switch
    {
        RegistrationIntakeStep.Identity => FormatIdentitySummary(dto),
        RegistrationIntakeStep.LegalResidence => FormatLegalResidenceSummary(dto),
        RegistrationIntakeStep.Address => FormatAddressSummary(dto),
        RegistrationIntakeStep.Household => FormatHouseholdSummary(dto),
        RegistrationIntakeStep.CivilStatus => FormatCivilStatusSummary(dto),
        RegistrationIntakeStep.BirthInformation => FormatBirthInformationSummary(dto),
        RegistrationIntakeStep.PoliceVerification => FormatPoliceSummary(dto),
        _ => string.Empty,
    };

    public static AppSeverity GetStatusSeverity(RegistrationIntakeStep step, RegistrationCaseDetailDto dto)
    {
        if (step == RegistrationIntakeStep.PoliceVerification && dto.ActivePoliceVerification is not null)
        {
            return AppSeverity.Warning;
        }

        if (step == RegistrationIntakeStep.BirthInformation
            && dto.Person?.BirthInformation is not null
            && !dto.Checklist.BirthEvidenceEstablished)
        {
            return AppSeverity.Warning;
        }

        return IsComplete(step, dto) ? AppSeverity.Success : AppSeverity.Neutral;
    }

    public static string GetStatusLabel(RegistrationIntakeStep step, RegistrationCaseDetailDto dto)
    {
        if (step == RegistrationIntakeStep.PoliceVerification && dto.ActivePoliceVerification is not null)
        {
            return "Awaiting";
        }

        return IsComplete(step, dto) ? "Complete" : "Pending";
    }

    private static string FormatIdentitySummary(RegistrationCaseDetailDto dto)
    {
        if (dto.Person is null)
        {
            return "Not recorded";
        }

        return $"{dto.Person.GivenName} {dto.Person.FamilyName} · born {dto.Person.BirthDate:d} · {dto.Person.Nationality}";
    }

    private static string FormatLegalResidenceSummary(RegistrationCaseDetailDto dto)
    {
        if (dto.ResidenceCategory is null)
        {
            return "Not classified";
        }

        var summary = FormatResidenceCategory(dto.ResidenceCategory.Value);

        if (dto.ResidencePermit is { } permit)
        {
            summary += $" · permit until {permit.ValidUntil:d}";
        }

        return summary;
    }

    private static string FormatAddressSummary(RegistrationCaseDetailDto dto)
    {
        if (dto.DeclaredAddress is null)
        {
            return "Not declared";
        }

        var address = dto.DeclaredAddress;
        var line = $"{address.Street} {address.HouseNumber}";
        if (!string.IsNullOrWhiteSpace(address.Box))
        {
            line += $"/{address.Box}";
        }

        line += $", {address.PostalCode} {address.Municipality}";

        if (dto.HousingSituation is { } housing)
        {
            line += $" · {FormatHousingSituation(housing)}";
        }

        return line;
    }

    private static string FormatHouseholdSummary(RegistrationCaseDetailDto dto)
    {
        var count = dto.HouseholdMembers.Count;
        return count switch
        {
            0 => "No members recorded",
            1 => "1 member",
            _ => $"{count} members",
        };
    }

    private static string FormatCivilStatusSummary(RegistrationCaseDetailDto dto)
    {
        if (dto.CivilStatus is null)
        {
            return "Not recorded";
        }

        var summary = FormatCivilStatus(dto.CivilStatus.Status);

        if (CivilStatusRecord.RequiresMarriageDetails(dto.CivilStatus.Status) &&
            !string.IsNullOrWhiteSpace(dto.CivilStatus.SpouseGivenName))
        {
            summary += $" · {dto.CivilStatus.SpouseGivenName} {dto.CivilStatus.SpouseFamilyName}".Trim();
        }

        if (dto.CivilStatus.MarriageRecognitionStatus == MarriageRecognitionStatus.PendingRecognition)
        {
            summary += " · recognition pending";
        }

        return summary;
    }

    private static string FormatBirthInformationSummary(RegistrationCaseDetailDto dto)
    {
        if (dto.Person?.BirthInformation is null)
        {
            return "Not recorded";
        }

        var birth = dto.Person.BirthInformation;
        var summary = birth.BirthPlace;

        if (!string.IsNullOrWhiteSpace(birth.BirthCountry))
        {
            summary += $", {birth.BirthCountry}";
        }

        if (!dto.Checklist.BirthEvidenceEstablished)
        {
            summary += " · certificate missing";
        }

        return summary;
    }

    private static string FormatPoliceSummary(RegistrationCaseDetailDto dto)
    {
        if (dto.ActivePoliceVerification is { } pending)
        {
            return $"Awaiting result · visit {pending.AttemptNumber}";
        }

        var latest = dto.PoliceVerificationHistory
            .OrderByDescending(v => v.AttemptNumber)
            .FirstOrDefault();

        if (latest?.Result is { } result)
        {
            return $"{FormatPoliceResult(result)} · visit {latest.AttemptNumber}";
        }

        return "No visits recorded";
    }

    private static string FormatResidenceCategory(ResidenceCategory category) => category switch
    {
        ResidenceCategory.EuCitizen => "EU citizen",
        ResidenceCategory.NonEuWorker => "Non-EU worker",
        ResidenceCategory.Student => "Student",
        ResidenceCategory.Refugee => "Refugee / temporary protection",
        _ => category.ToString(),
    };

    private static string FormatHousingSituation(HousingSituation situation) => situation switch
    {
        HousingSituation.Owner => "Owner",
        HousingSituation.Tenant => "Tenant",
        HousingSituation.Lodging => "Lodging",
        HousingSituation.StudentHousing => "Student housing",
        HousingSituation.Shelter => "Shelter",
        _ => situation.ToString(),
    };

    private static string FormatCivilStatus(CivilStatus status) => status switch
    {
        CivilStatus.Single => "Single",
        CivilStatus.Married => "Married",
        CivilStatus.Divorced => "Divorced",
        CivilStatus.Widowed => "Widowed",
        CivilStatus.Separated => "Separated",
        CivilStatus.RegisteredPartnership => "Registered partnership",
        _ => status.ToString(),
    };

    private static string FormatPoliceResult(PoliceVerificationResult result) => result switch
    {
        PoliceVerificationResult.Confirmed => "Confirmed",
        PoliceVerificationResult.NotFound => "Not found",
        PoliceVerificationResult.AddressIncorrect => "Address incorrect",
        PoliceVerificationResult.MailboxOnly => "Mailbox only",
        PoliceVerificationResult.EmptyDwelling => "Empty dwelling",
        PoliceVerificationResult.RefusedAccess => "Refused access",
        PoliceVerificationResult.Incomplete => "Incomplete",
        _ => result.ToString(),
    };
}
