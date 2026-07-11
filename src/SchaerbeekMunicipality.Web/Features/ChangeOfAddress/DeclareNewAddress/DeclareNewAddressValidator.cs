using FluentValidation;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.DeclareNewAddress;

public sealed class DeclareNewAddressValidator : AbstractValidator<DeclareNewAddressRequest>
{
    public DeclareNewAddressValidator()
    {
        RuleFor(x => x.Street).BelgianStreet();
        RuleFor(x => x.HouseNumber).BelgianHouseNumber();
        RuleFor(x => x.PostalCode).BelgianPostalCode();
        RuleFor(x => x.Municipality).BelgianMunicipality();
        RuleFor(x => x.HousingSituation).IsInEnum();
        RuleFor(x => x.EffectiveDate).NotEmpty();
    }
}
