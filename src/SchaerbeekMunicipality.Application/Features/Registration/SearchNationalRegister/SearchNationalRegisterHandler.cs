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

        IReadOnlyList<(NationalRegisterMatch Match, Person? Person)> pageSource;
        int totalCount;

        if (request.ExcludeDeceased)
        {
            var living = new List<(NationalRegisterMatch Match, Person? Person)>(matches.Count);
            foreach (var match in matches)
            {
                var linkedPerson = await ResolveLinkedPersonAsync(match, cancellationToken);
                if (linkedPerson?.IsDeceased == true)
                    continue;

                living.Add((match, linkedPerson));
            }

            totalCount = living.Count;
            pageSource = living
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
        }
        else
        {
            totalCount = matches.Count;
            var pageMatches = matches
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var resolved = new List<(NationalRegisterMatch Match, Person? Person)>(pageMatches.Count);
            foreach (var match in pageMatches)
            {
                var linkedPerson = await ResolveLinkedPersonAsync(match, cancellationToken);
                resolved.Add((match, linkedPerson));
            }

            pageSource = resolved;
        }

        var dtos = pageSource
            .Select(item =>
            {
                var (match, linkedPerson) = item;
                var isRegistered = linkedPerson?.NationalRegisterNumber is not null;
                return new NationalRegisterMatchDto(
                    match.RegisterPersonId.Value,
                    match.GivenName,
                    match.FamilyName,
                    match.BirthDate,
                    match.Nationality,
                    match.BisNumber,
                    match.NationalRegisterNumber,
                    match.MatchScore,
                    match.MatchReason,
                    isRegistered,
                    linkedPerson?.Id.Value,
                    linkedPerson?.IsDeceased == true);
            })
            .ToList();

        return new SearchNationalRegisterResponse(dtos, totalCount, request.Page, request.PageSize);
    }

    private async Task<Person?> ResolveLinkedPersonAsync(
        NationalRegisterMatch match,
        CancellationToken cancellationToken)
    {
        if (match.NationalRegisterNumber is null)
            return await personRepository.GetByRegisterRecordIdAsync(match.RegisterPersonId, cancellationToken);

        var person = await personRepository.GetByRegisterRecordIdAsync(
            match.RegisterPersonId,
            cancellationToken);

        if (person is null)
        {
            var registerPerson = await nationalRegisterRepository.GetByIdAsync(
                match.RegisterPersonId,
                cancellationToken);

            if (registerPerson?.NationalRegisterNumber is { } nrNumber)
                person = await personRepository.GetByNationalRegisterNumberAsync(nrNumber, cancellationToken);
        }

        return person;
    }
}
