using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;

public sealed class OpenDeathDeclarationCaseValidator : AbstractValidator<OpenDeathDeclarationCaseRequest>
{
    public OpenDeathDeclarationCaseValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
    }
}
