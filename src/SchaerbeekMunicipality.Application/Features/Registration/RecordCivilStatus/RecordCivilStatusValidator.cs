using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordCivilStatus;

public sealed class RecordCivilStatusValidator : AbstractValidator<RecordCivilStatusRequest>
{
    public RecordCivilStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("A valid civil status is required.");

        When(x => CivilStatusRecord.RequiresMarriageDetails(x.Status), () =>
        {
            RuleFor(x => x.SpouseGivenName)
                .NotEmpty()
                .WithMessage("Spouse given name is required for this civil status.");

            RuleFor(x => x.SpouseFamilyName)
                .NotEmpty()
                .WithMessage("Spouse family name is required for this civil status.");

            RuleFor(x => x.MarriageDate)
                .NotNull()
                .WithMessage("Marriage date is required for this civil status.");
        });
    }
}