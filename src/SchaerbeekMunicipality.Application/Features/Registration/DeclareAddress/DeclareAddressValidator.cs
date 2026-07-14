using FluentValidation;
using SchaerbeekMunicipality.Application.Validation;

namespace SchaerbeekMunicipality.Application.Features.Registration.DeclareAddress;

public sealed class DeclareAddressValidator : AbstractValidator<DeclareAddressRequest>
{
    public DeclareAddressValidator()
    {
        RuleFor(x => x.Street)
            .BelgianStreet()
            .WithMessage("Street is required.");

        RuleFor(x => x.HouseNumber)
            .BelgianHouseNumber()
            .WithMessage("House number is required.");

        RuleFor(x => x.PostalCode).SchaerbeekPostalCode();
        RuleFor(x => x.Municipality).SchaerbeekMunicipality();
    }
}