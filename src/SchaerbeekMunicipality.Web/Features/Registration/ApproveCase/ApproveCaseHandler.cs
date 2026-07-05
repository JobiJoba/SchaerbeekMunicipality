using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.ApproveCase;

public sealed class ApproveCaseHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
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

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

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
