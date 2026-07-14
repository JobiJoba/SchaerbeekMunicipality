using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.LinkParent;

public sealed record LinkParentRequest(Guid RegisterPersonId, ParentRole Role);

public sealed class LinkParentValidator : AbstractValidator<LinkParentRequest>
{
    public LinkParentValidator()
    {
        RuleFor(x => x.RegisterPersonId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
    }
}