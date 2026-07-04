using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

public sealed class RecordIdentityValidator : AbstractValidator<RecordIdentityRequest>
{
    public RecordIdentityValidator()
    {
        RuleFor(x => x.GivenName)
            .NotEmpty()
            .WithMessage("Given name is required.");

        RuleFor(x => x.FamilyName)
            .NotEmpty()
            .WithMessage("Family name is required.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Birth date must be in the past.");

        RuleFor(x => x.Nationality)
            .NotEmpty()
            .WithMessage("Nationality is required.");
    }
}
