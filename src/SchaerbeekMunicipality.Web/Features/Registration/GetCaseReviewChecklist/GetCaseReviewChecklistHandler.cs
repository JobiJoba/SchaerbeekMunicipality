using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetCaseReviewChecklist;

public sealed class GetCaseReviewChecklistHandler(
    RegistrationCaseGuard caseGuard,
    RegistrationCaseAuthorization authorization,
    ICurrentOfficer currentOfficer,
    IPersonRepository personRepository)
{
    public async Task<GetCaseReviewChecklistResponse?> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var registrationCase = await caseGuard.GetForViewAsync(caseId, cancellationToken);

        string? nationality = null;
        if (registrationCase.PersonId is { } personId)
        {
            var person = await personRepository.GetByIdAsync(personId, cancellationToken);
            nationality = person?.Nationality;
        }

        var suggested = RegisterTargetResolver.Suggest(registrationCase.ResidenceCategory, nationality);

        return new GetCaseReviewChecklistResponse(
            registrationCase.Id.Value,
            registrationCase.Status.ToString(),
            registrationCase.IsReadyForApproval,
            suggested?.ToString(),
            CaseReviewChecklistMapper.BuildQuestions(registrationCase.Checklist));
    }
}
