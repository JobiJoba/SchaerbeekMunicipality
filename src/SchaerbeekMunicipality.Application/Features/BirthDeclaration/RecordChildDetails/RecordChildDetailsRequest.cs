using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;

public sealed record RecordChildDetailsRequest(
    string GivenNames,
    string FamilyName,
    NewbornSex Sex,
    DateOnly DateOfBirth,
    TimeOnly? TimeOfBirth,
    string PlaceOfBirth);

public sealed class RecordChildDetailsValidator : AbstractValidator<RecordChildDetailsRequest>
{
    public RecordChildDetailsValidator()
    {
        RuleFor(x => x.GivenNames).NotEmpty().MaximumLength(256);
        RuleFor(x => x.FamilyName).NotEmpty().MaximumLength(256);
        RuleFor(x => x.PlaceOfBirth).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Sex).IsInEnum();
    }
}
