using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.LinkExistingPerson;

public sealed class LinkExistingPersonValidator : AbstractValidator<LinkExistingPersonRequest>
{
    public LinkExistingPersonValidator()
    {
        RuleFor(x => x.RegisterPersonId).NotEmpty();
    }
}