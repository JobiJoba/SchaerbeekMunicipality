using FluentValidation;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.DeathDeclaration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.DeathDeclaration.SuspendDeathDeclaration;

public sealed record SuspendDeathDeclarationRequest(SuspensionReason Reason, string? Notes);

public sealed class SuspendDeathDeclarationValidator : AbstractValidator<SuspendDeathDeclarationRequest>
{
    public SuspendDeathDeclarationValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
    }
}

public sealed record SuspendDeathDeclarationResponse(Guid CaseId, string Status);

public sealed class SuspendDeathDeclarationHandler(
    DeathDeclarationCaseGuard caseGuard,
    IDeathDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<SuspendDeathDeclarationRequest> validator)
{
    public async Task<SuspendDeathDeclarationResponse> Handle(
        DeathDeclarationCaseId caseId,
        SuspendDeathDeclarationRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
            throw new UnauthorizedAccessException("Only population officers can suspend death declaration cases.");

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var deathDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SuspendDeathDeclaration),
            cancellationToken);

        deathDeclarationCase.Suspend(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SuspendDeathDeclarationResponse(
            deathDeclarationCase.Id.Value,
            deathDeclarationCase.Status.ToString());
    }
}
