using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;
using SchaerbeekMunicipality.Application.Auth;

namespace SchaerbeekMunicipality.Application.Features.Registration.ApproveCase;

public sealed class ApproveCaseHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    CaseAuditRecorder auditRecorder,
    ICurrentOfficer currentOfficer,
    TimeProvider timeProvider,
    IValidator<ApproveCaseRequest> validator)
{
    public async Task<ApproveCaseResponse> Handle(
        RegistrationCaseId caseId,
        ApproveCaseRequest request,
        CancellationToken cancellationToken)
    {
        if (!currentOfficer.CanApproveRegistration)
        {
            throw new UnauthorizedAccessException("Only population officers can approve registration cases.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(ApproveCase),
            cancellationToken);

        var evaluation = await exceptionEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        if (evaluation.IllegalStayDetected)
        {
            throw new InvalidRegistrationTransitionException(
                "Cannot approve a case with no legal residence basis — reject and refer to Immigration Office.");
        }

        if (evaluation.MarriageRecognitionBlocking)
        {
            throw new InvalidRegistrationTransitionException(
                "Cannot approve while foreign marriage recognition is pending.");
        }

        if (!registrationCase.IsReadyForApproval)
        {
            throw new InvalidRegistrationTransitionException(
                "Cannot approve the case until all review checklist items are satisfied.");
        }

        string? nationality = null;
        if (registrationCase.PersonId is { } personId)
        {
            var person = await personRepository.GetByIdAsync(personId, cancellationToken);
            nationality = person?.Nationality;
        }

        var officer = OfficerId.From(currentOfficer.OfficerId);
        registrationCase.Approve(
            officer,
            request.RegisterTarget,
            nationality,
            timeProvider.GetUtcNow());

        await auditRecorder.RecordAsync(
            caseId,
            CaseAuditAction.CaseApproved,
            $"Register: {request.RegisterTarget}",
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new ApproveCaseResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            request.RegisterTarget.ToString());
    }
}
