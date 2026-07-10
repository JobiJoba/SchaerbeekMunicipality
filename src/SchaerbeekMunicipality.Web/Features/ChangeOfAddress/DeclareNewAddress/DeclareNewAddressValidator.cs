using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.DeclareNewAddress;

public sealed class DeclareNewAddressValidator : AbstractValidator<DeclareNewAddressRequest>
{
    public DeclareNewAddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(256);
        RuleFor(x => x.HouseNumber).NotEmpty().MaximumLength(16);
        RuleFor(x => x.PostalCode).NotEmpty().Length(4);
        RuleFor(x => x.Municipality).NotEmpty().MaximumLength(128);
        RuleFor(x => x.HousingSituation).IsInEnum();
        RuleFor(x => x.EffectiveDate).NotEmpty();
    }
}
