using FluentValidation;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordBirthInformation;

public sealed class RecordBirthInformationRequestValidator : AbstractValidator<RecordBirthInformationRequest>
{
    public RecordBirthInformationRequestValidator()
    {
        RuleFor(x => x.BirthPlace)
            .NotEmpty()
            .MaximumLength(128);
        RuleFor(x => x.BirthCountry)
            .MaximumLength(64);
    }
}