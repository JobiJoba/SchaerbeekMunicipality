using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;

public sealed class SearchPersonFileHandler(
    PersonFileAuthorization authorization,
    ICurrentOfficer currentOfficer,
    SearchNationalRegisterHandler searchNationalRegisterHandler,
    IPersonRepository personRepository)
{
    public async Task<SearchPersonFileResponse> Handle(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken)
    {
        authorization.EnsureCanView(currentOfficer);

        var searchResult = await searchNationalRegisterHandler.Handle(request, cancellationToken);
        var matches = new List<PersonFileSearchMatchDto>(searchResult.Matches.Count);

        foreach (var match in searchResult.Matches)
        {
            var personId = await ResolvePersonIdAsync(match, cancellationToken);
            matches.Add(new PersonFileSearchMatchDto(
                match.RegisterPersonId,
                match.GivenName,
                match.FamilyName,
                match.BirthDate,
                match.Nationality,
                match.BisNumber,
                match.NationalRegisterNumber,
                match.MatchScore,
                match.MatchReason,
                match.IsRegisteredInPopulation,
                personId,
                personId is not null));
        }

        return new SearchPersonFileResponse(matches);
    }

    private async Task<Guid?> ResolvePersonIdAsync(
        NationalRegisterMatchDto match,
        CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByRegisterRecordIdAsync(
            new NationalRegisterPersonId(match.RegisterPersonId),
            cancellationToken);

        if (person is null && match.NationalRegisterNumber is not null)
        {
            try
            {
                var nr = NationalRegisterNumber.Create(match.NationalRegisterNumber);
                person = await personRepository.GetByNationalRegisterNumberAsync(nr, cancellationToken);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        return person?.Id.Value;
    }
}
