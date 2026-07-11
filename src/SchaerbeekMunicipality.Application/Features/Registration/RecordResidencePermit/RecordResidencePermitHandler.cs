using FluentValidation;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.RecordResidencePermit;

public sealed class RecordResidencePermitHandler(
    RegistrationCaseGuard caseGuard,
    IResidencePermitRepository permitRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    TimeProvider timeProvider,
    IValidator<RecordResidencePermitRequest> validator)
{
    public async Task<RecordResidencePermitResponse> Handle(
        RegistrationCaseId caseId,
        RecordResidencePermitRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordResidencePermit),
            cancellationToken);

        if (registrationCase.ResidenceCategory is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Residence category must be set before recording a permit.");
        }

        registrationCase.EnsureIntakeDataEditable(nameof(RecordResidencePermit));

        if (!registrationCase.Checklist.IdentityEstablished)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity must be recorded before residence information can be captured.");
        }

        var details = new ResidencePermitDetails(
            request.PermitType,
            request.ValidFrom,
            request.ValidUntil,
            request.CardNumber,
            request.IssuingAuthority);

        var recordedAt = timeProvider.GetUtcNow();
        var existingPermit = await permitRepository.GetByCaseIdAsync(caseId, cancellationToken);

        ResidencePermit permit;
        if (existingPermit is null)
        {
            permit = ResidencePermit.Create(caseId, details, recordedAt);
            await permitRepository.AddAsync(permit, cancellationToken);
        }
        else
        {
            existingPermit.Update(details, recordedAt);
            permit = existingPermit;
        }

        var evaluation = await exceptionEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken,
            permitOverride: permit);

        await permitRepository.SaveChangesAsync(cancellationToken);

        return new RecordResidencePermitResponse(
            registrationCase.Id.Value,
            permit.Id.Value,
            registrationCase.Checklist.LegalResidenceEstablished,
            evaluation.PolicyResult.IsValid ? null : evaluation.PolicyResult.FailureReason);
    }
}
