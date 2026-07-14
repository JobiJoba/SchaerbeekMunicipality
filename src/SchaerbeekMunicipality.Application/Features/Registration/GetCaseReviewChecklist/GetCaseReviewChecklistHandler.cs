using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Immigration;
using SchaerbeekMunicipality.Domain.Immigration.Policies;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.GetCaseReviewChecklist;

public sealed class GetCaseReviewChecklistHandler(
    RegistrationCaseGuard caseGuard,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository,
    IAdministrativeDocumentRepository documentRepository,
    IResidencePermitRepository permitRepository,
    ResidencePolicyEvaluator policyEvaluator)
{
    public async Task<GetCaseReviewChecklistResponse?> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var registrationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        Person? person = null;
        if (registrationCase.PersonId is { } personId)
            person = await personRepository.GetByIdAsync(personId, cancellationToken);

        var permit = await permitRepository.GetByCaseIdAsync(caseId, cancellationToken);
        var documents = await documentRepository.ListByRegistrationCaseIdAsync(caseId, cancellationToken);
        var documentTypes = documents.Select(d => d.DocumentType).ToList();
        var policyResult = policyEvaluator.Evaluate(registrationCase, permit, documentTypes);
        var exceptionState = RegistrationExceptionStateCalculator.Calculate(
            registrationCase,
            person,
            documentTypes,
            policyResult);

        var suggested = RegisterTargetResolver.Suggest(
            registrationCase.ResidenceCategory,
            person?.Nationality,
            registrationCase.ImmigrationDecision is not null);

        return new GetCaseReviewChecklistResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            exceptionState.IsReadyForApproval,
            suggested?.ToString(),
            CaseReviewChecklistMapper.BuildQuestions(
                registrationCase.Checklist,
                exceptionState.BirthEvidenceEstablished,
                exceptionState.MarriageRecognitionBlocking,
                exceptionState.IllegalStayDetected));
    }
}