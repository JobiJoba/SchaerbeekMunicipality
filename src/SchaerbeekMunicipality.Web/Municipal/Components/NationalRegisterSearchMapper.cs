using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Web.Municipal.Components;

internal static class NationalRegisterSearchMapper
{
    public static SearchNationalRegisterRequest ToRequest(
        NrSearchFormCriteria criteria,
        NationalRegisterSearchEligibility eligibility = NationalRegisterSearchEligibility.All)
    {
        return new SearchNationalRegisterRequest(
            criteria.GivenName,
            criteria.FamilyName,
            criteria.BirthDate,
            criteria.Page,
            criteria.PageSize,
            eligibility);
    }

    public static NationalRegisterSearchResult ToResult(SearchNationalRegisterResponse response)
    {
        return new NationalRegisterSearchResult(
            response.Matches.Select(FromDto).ToList(),
            response.TotalCount,
            response.Page,
            response.PageSize);
    }

    public static NationalRegisterSearchResult ToResult(SearchPersonFileResponse response)
    {
        return new NationalRegisterSearchResult(
            response.Matches.Select(FromPersonFileDto).ToList(),
            response.TotalCount,
            response.Page,
            response.PageSize);
    }

    public static NationalRegisterSearchMatch FromDto(NationalRegisterMatchDto match)
    {
        return new NationalRegisterSearchMatch(
            match.RegisterPersonId,
            match.GivenName,
            match.FamilyName,
            match.BirthDate,
            match.BisNumber,
            match.NationalRegisterNumber,
            match.MatchScore,
            match.MatchReason,
            match.IsRegisteredInPopulation,
            match.PersonId,
            match.IsDeceased);
    }

    private static NationalRegisterSearchMatch FromPersonFileDto(PersonFileSearchMatchDto match)
    {
        return new NationalRegisterSearchMatch(
            match.RegisterPersonId,
            match.GivenName,
            match.FamilyName,
            match.BirthDate,
            match.BisNumber,
            match.NationalRegisterNumber,
            match.MatchScore,
            match.MatchReason,
            match.IsRegisteredInPopulation,
            match.PersonId,
            match.IsDeceased);
    }
}
