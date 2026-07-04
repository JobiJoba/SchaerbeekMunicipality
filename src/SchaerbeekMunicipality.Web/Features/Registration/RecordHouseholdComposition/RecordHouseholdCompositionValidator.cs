using FluentValidation;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordHouseholdComposition;

public sealed class RecordHouseholdCompositionValidator : AbstractValidator<RecordHouseholdCompositionRequest>
{
    public RecordHouseholdCompositionValidator()
    {
        RuleForEach(x => x.Members).ChildRules(member =>
        {
            member.RuleFor(m => m.GivenName)
                .NotEmpty()
                .WithMessage("Given name is required for each household member.");

            member.RuleFor(m => m.FamilyName)
                .NotEmpty()
                .WithMessage("Family name is required for each household member.");

            member.RuleFor(m => m.BirthDate)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Birth date cannot be in the future.");

            member.RuleFor(m => m.Role)
                .IsInEnum()
                .WithMessage("A valid household role is required.");
        });
    }
}
