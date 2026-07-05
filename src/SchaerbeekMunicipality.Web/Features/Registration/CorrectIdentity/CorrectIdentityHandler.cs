using FluentValidation;
using Microsoft.Extensions.Logging;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Features.Registration;
using SchaerbeekMunicipality.Web.Features.Registration.RecordIdentity;

namespace SchaerbeekMunicipality.Web.Features.Registration.CorrectIdentity;

public sealed class CorrectIdentityHandler(
    RegistrationCaseGuard caseGuard,
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository,
    RegistrationResidenceEvaluator residenceEvaluator,
    IValidator<RecordIdentityRequest> validator,
    ILogger<CorrectIdentityHandler> logger)
{
    public async Task<CorrectIdentityResponse> Handle(
        RegistrationCaseId caseId,
        RecordIdentityRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var registrationCase = await caseGuard.GetForEditAsync(
            caseId,
            nameof(CorrectIdentity),
            cancellationToken);

        if (registrationCase.PersonId is null)
        {
            throw new InvalidRegistrationTransitionException(
                "Identity has not been recorded for this case.");
        }

        var person = await personRepository.GetForUpdateAsync(registrationCase.PersonId.Value, cancellationToken)
            ?? throw new KeyNotFoundException(
                $"Person '{registrationCase.PersonId.Value}' was not found.");

        var identity = new IdentityDetails(
            request.GivenName,
            request.FamilyName,
            request.BirthDate,
            request.Nationality);

        registrationCase.CorrectIdentity(person, identity);
        registrationCase.RefreshRegisterDeterminability(request.Nationality);

        var policyResult = await residenceEvaluator.EvaluateAndApplyAsync(
            registrationCase,
            cancellationToken);

        await caseRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Corrected identity for case {CaseId} (person {PersonId})",
            caseId.Value,
            person.Id.Value);

        return new CorrectIdentityResponse(
            registrationCase.Id.Value,
            person.Id.Value,
            registrationCase.Checklist.IdentityEstablished,
            registrationCase.Checklist.LegalResidenceEstablished,
            policyResult.IsValid ? null : policyResult.FailureReason);
    }
}
