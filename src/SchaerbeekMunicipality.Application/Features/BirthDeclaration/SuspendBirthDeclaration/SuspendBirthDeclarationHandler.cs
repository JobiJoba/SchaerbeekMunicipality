using FluentValidation;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.BirthDeclaration.SuspendBirthDeclaration;

public sealed record SuspendBirthDeclarationRequest(SuspensionReason Reason, string? Notes);

public sealed class SuspendBirthDeclarationValidator : AbstractValidator<SuspendBirthDeclarationRequest>
{
    public SuspendBirthDeclarationValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
    }
}

public sealed record SuspendBirthDeclarationResponse(Guid CaseId, string Status);

public sealed class SuspendBirthDeclarationHandler(
    BirthDeclarationCaseGuard caseGuard,
    IBirthDeclarationCaseRepository caseRepository,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<SuspendBirthDeclarationRequest> validator)
{
    public async Task<SuspendBirthDeclarationResponse> Handle(
        BirthDeclarationCaseId caseId,
        SuspendBirthDeclarationRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can suspend birth declaration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var birthDeclarationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SuspendBirthDeclaration),
            cancellationToken);

        birthDeclarationCase.Suspend(
            OfficerId.From(currentOfficer.OfficerId),
            request.Reason,
            request.Notes,
            timeProvider.GetUtcNow());

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SuspendBirthDeclarationResponse(
            birthDeclarationCase.Id.Value,
            birthDeclarationCase.Status.ToString());
    }
}
