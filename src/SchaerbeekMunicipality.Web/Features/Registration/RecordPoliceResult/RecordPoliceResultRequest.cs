using FluentValidation;
using SchaerbeekMunicipality.Domain.Police;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;

public sealed record RecordPoliceResultRequest(
    PoliceVerificationResult Result,
    string? OfficerNotes);

public sealed class RecordPoliceResultValidator : AbstractValidator<RecordPoliceResultRequest>
{
    public RecordPoliceResultValidator()
    {
        RuleFor(r => r.Result)
            .IsInEnum();

        RuleFor(r => r.OfficerNotes)
            .MaximumLength(2000);
    }
}
