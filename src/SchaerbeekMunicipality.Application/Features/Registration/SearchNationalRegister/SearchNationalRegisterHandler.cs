using FluentValidation;
using SchaerbeekMunicipality.Domain.Identity;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed class SearchNationalRegisterHandler(
    INationalRegisterRepository nationalRegisterRepository,
    IPersonRepository personRepository,
    IValidator<SearchNationalRegisterRequest> validator)
{
    public async Task<SearchNationalRegisterResponse> Handle(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var criteria = new NationalRegisterSearchCriteria(
            request.GivenName,
            request.FamilyName,
            request.BirthDate);

        var matches = await nationalRegisterRepository.SearchAsync(criteria, cancellationToken);
        var totalCount = matches.Count;
        var pageMatches = matches
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = new List<NationalRegisterMatchDto>(pageMatches.Count);
        foreach (var match in pageMatches)
        {
            var isRegistered = await IsRegisteredInPopulationAsync(match, cancellationToken);
            dtos.Add(new NationalRegisterMatchDto(
                match.RegisterPersonId.Value,
                match.GivenName,
                match.FamilyName,
                match.BirthDate,
                match.Nationality,
                match.BisNumber,
                match.NationalRegisterNumber,
                match.MatchScore,
                match.MatchReason,
                isRegistered));
        }

        return new SearchNationalRegisterResponse(dtos, totalCount, request.Page, request.PageSize);
    }

    private async Task<bool> IsRegisteredInPopulationAsync(
        NationalRegisterMatch match,
        CancellationToken cancellationToken)
    {
        if (match.NationalRegisterNumber is null)
        {
            return false;
        }

        var person = await personRepository.GetByRegisterRecordIdAsync(
            match.RegisterPersonId,
            cancellationToken);

        if (person is null)
        {
            var registerPerson = await nationalRegisterRepository.GetByIdAsync(
                match.RegisterPersonId,
                cancellationToken);

            if (registerPerson?.NationalRegisterNumber is { } nrNumber)
            {
                person = await personRepository.GetByNationalRegisterNumberAsync(nrNumber, cancellationToken);
            }
        }

        return person?.NationalRegisterNumber is not null;
    }
}
