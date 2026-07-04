using FluentValidation;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordResidencePermit;

public sealed class RecordResidencePermitHandler(
    IRegistrationCaseRepository caseRepository,
    IResidencePermitRepository permitRepository,
    RegistrationResidenceEvaluator residenceEvaluator,
    TimeProvider timeProvider,
    IValidator<RecordResidencePermitRequest> validator)
{
    public async Task<RecordResidencePermitResponse> Handle(
        RegistrationCaseId caseId,
        RecordResidencePermitRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        if (registrationCase.ResidenceCategory is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Residence category must be set before recording a permit.");
        }

        if (registrationCase.Status != RegistrationCaseStatus.Intake)
        {
            throw new InvalidRegistrationTransitionException(
                $"Cannot record a residence permit while the case is in status '{registrationCase.Status}'.");
        }

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

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken,
            permit);

        await permitRepository.SaveChangesAsync(cancellationToken);

        return new RecordResidencePermitResponse(
            registrationCase.Id.Value,
            permit.Id.Value,
            registrationCase.Checklist.LegalResidenceEstablished,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}
