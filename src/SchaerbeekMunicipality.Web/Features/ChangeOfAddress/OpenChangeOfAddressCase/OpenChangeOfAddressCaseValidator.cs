using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.ChangeOfAddress.OpenChangeOfAddressCase;

public sealed class OpenChangeOfAddressCaseValidator : AbstractValidator<OpenChangeOfAddressCaseRequest>
{
    public OpenChangeOfAddressCaseValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
    }
}
