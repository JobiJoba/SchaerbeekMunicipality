using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Features.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.SetResidenceCategory;

public sealed class SetResidenceCategoryHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    RegistrationExceptionEvaluator exceptionEvaluator,
    IValidator<SetResidenceCategoryRequest> validator)
{
    public async Task<SetResidenceCategoryResponse> Handle(
        RegistrationCaseId caseId,
        SetResidenceCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(SetResidenceCategory),
            cancellationToken);

        registrationCase.SetResidenceCategory(request.Category);

        var evaluation = await exceptionEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SetResidenceCategoryResponse(
            registrationCase.Id.Value,
            request.Category,
            registrationCase.Checklist.LegalResidenceEstablished,
            evaluation.PolicyResult.IsValid ? null : evaluation.PolicyResult.FailureReason);
    }
}
