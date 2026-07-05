using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.GetCaseReviewChecklist;

public sealed class GetCaseReviewChecklistHandler(
    IRegistrationCaseRepository caseRepository,
    IPersonRepository personRepository)
{
    public async Task<GetCaseReviewChecklistResponse?> Handle(
        RegistrationCaseId caseId,
        CancellationToken cancellationToken)
    {
        var registrationCase = await caseRepository.GetByIdAsync(caseId, cancellationToken);
        if (registrationCase is null)
        {
            return null;
        }

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
