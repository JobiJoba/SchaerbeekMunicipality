using FluentValidation;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

public sealed class SetResidenceCategoryHandler(
    IRegistrationCaseRepository caseRepository,
    RegistrationResidenceEvaluator residenceEvaluator,
    IValidator<SetResidenceCategoryRequest> validator)
{
    public async Task<SetResidenceCategoryResponse> Handle(
        RegistrationCaseId caseId,
        SetResidenceCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken)
            ?? throw new KeyNotFoundException($"Registration case '{caseId}' was not found.");

        registrationCase.SetResidenceCategory(request.Category);

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        return new SetResidenceCategoryResponse(
            registrationCase.Id.Value,
            request.Category,
            registrationCase.Checklist.LegalResidenceEstablished,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}
