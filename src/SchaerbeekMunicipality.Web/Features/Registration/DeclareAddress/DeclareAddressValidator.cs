using FluentValidation;
using SchaerbeekMunicipality.Domain.ReferenceData;

namespace SchaerbeekMunicipality.Web.Features.Registration.DeclareAddress;

public sealed class DeclareAddressValidator : AbstractValidator<DeclareAddressRequest>
{
    public DeclareAddressValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required.");

        RuleFor(x => x.HouseNumber)
            .NotEmpty()
            .WithMessage("House number is required.");

        RuleFor(x => x.PostalCode)
            .Equal(SchaerbeekCommune.PostalCode)
            .WithMessage($"Postal code must be {SchaerbeekCommune.PostalCode} for registration at Schaerbeek.");

        RuleFor(x => x.Municipality)
            .Must(m => m.Trim().Equals(SchaerbeekCommune.Name, StringComparison.OrdinalIgnoreCase))
            .WithMessage($"Municipality must be {SchaerbeekCommune.Name} for registration at this desk.");
    }
}
