using FluentValidation;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.RecordImmigrationDecision;

public sealed class RecordImmigrationDecisionHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    RegistrationResidenceEvaluator residenceEvaluator,
    IValidator<RecordImmigrationDecisionRequest> validator)
{
    public async Task<RecordImmigrationDecisionResponse> Handle(
        RegistrationCaseId caseId,
        RecordImmigrationDecisionRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(RecordImmigrationDecision),
            cancellationToken);

        registrationCase.RecordImmigrationDecision(
            new ImmigrationDecisionDetails(request.ReferenceNumber, request.DecisionDate));

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new RecordImmigrationDecisionResponse(
            registrationCase.Id.Value,
            request.ReferenceNumber,
            registrationCase.Checklist.LegalResidenceEstablished,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}
