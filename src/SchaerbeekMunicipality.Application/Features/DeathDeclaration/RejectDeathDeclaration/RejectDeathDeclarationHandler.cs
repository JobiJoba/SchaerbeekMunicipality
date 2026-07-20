using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.RejectDeathDeclaration;

public sealed record RejectDeathDeclarationRequest(
    DeathDeclarationRejectionReason Reason,
    string? Notes);

public sealed class RejectDeathDeclarationValidator : AbstractValidator<RejectDeathDeclarationRequest>
{
    public RejectDeathDeclarationValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
    }
}

public sealed record RejectDeathDeclarationResponse(Guid CaseId, string Status, string Reason);

public sealed class RejectDeathDeclarationHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<RejectDeathDeclarationRequest> validator)
{
    public async Task<RejectDeathDeclarationResponse> Handle(
        DeathDeclarationCaseId caseId,
        RejectDeathDeclarationRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can reject death declaration cases.");

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RejectDeathDeclaration),
            cancellationToken);

        deathDeclarationCase.Reject(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RejectDeathDeclarationResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString(),
            request.Reason.ToString());
    }
}
