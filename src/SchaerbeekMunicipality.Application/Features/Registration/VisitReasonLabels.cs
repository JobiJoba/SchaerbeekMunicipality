using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration;

public static class VisitReasonLabels
{
    public static string For(VisitReason reason)
    {
        return reason switch
        {
            VisitReason.FirstRegistration => "First registration",
            VisitReason.ChangeOfAddress => "Change of address",
            VisitReason.EuCitizenRegistration => "EU citizen registration",
            VisitReason.BirthDeclaration => "Birth declaration",
            VisitReason.PassportRenewal => "Passport renewal",
            _ => reason.ToString()
        };
    }

    public static string Description(VisitReason reason)
    {
        return reason switch
        {
            VisitReason.FirstRegistration =>
                "A person registering at an address in Schaerbeek for the first time.",
            VisitReason.ChangeOfAddress =>
                "An existing register record moving to a new address within the municipality.",
            VisitReason.EuCitizenRegistration =>
                "An EU citizen establishing legal residence in Belgium.",
            VisitReason.BirthDeclaration =>
                "Declaring a newborn for registration in the population register.",
            VisitReason.PassportRenewal =>
                "Applying for a Belgian passport or national ID card for a registered resident.",
            _ => string.Empty
        };
    }
}