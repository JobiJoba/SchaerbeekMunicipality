using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.RejectBirthDeclaration;

public sealed record RejectBirthDeclarationRequest(
    BirthDeclarationRejectionReason Reason,
    string? Notes);

public sealed class RejectBirthDeclarationValidator : AbstractValidator<RejectBirthDeclarationRequest>
{
    public RejectBirthDeclarationValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
    }
}

public sealed record RejectBirthDeclarationResponse(Guid CaseId, string Status, string Reason);

public sealed class RejectBirthDeclarationHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectBirthDeclarationRequest> validator)
{
    public async Task<RejectBirthDeclarationResponse> Handle(
        BirthDeclarationCaseId caseId,
        RejectBirthDeclarationRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can reject birth declaration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RejectBirthDeclaration),
            cancellationToken);

        birthDeclarationCase.Reject(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectBirthDeclarationResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString(),
            request.Reason.ToString());
    }
}
