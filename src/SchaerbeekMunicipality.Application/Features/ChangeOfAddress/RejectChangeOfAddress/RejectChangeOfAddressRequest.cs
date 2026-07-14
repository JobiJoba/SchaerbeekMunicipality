using FluentValidation;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Application.Features.ChangeOfAddress.RejectChangeOfAddress;

public sealed record RejectChangeOfAddressRequest(
    ChangeOfAddressRejectionReason Reason,
    string? Notes);

public sealed class RejectChangeOfAddressValidator : AbstractValidator<RejectChangeOfAddressRequest>
{
    public RejectChangeOfAddressValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
    }
}

public sealed record RejectChangeOfAddressResponse(Guid CaseId, string Status, string Reason);