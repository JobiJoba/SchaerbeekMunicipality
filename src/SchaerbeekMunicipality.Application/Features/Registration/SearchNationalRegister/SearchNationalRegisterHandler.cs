using FluentValidation;
using SchaerbeekMunicipality.Domain.NationalRegister;

namespace SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

public sealed class SearchNationalRegisterHandler(
    INationalRegisterRepository nationalRegisterRepository,
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

        return new SearchNationalRegisterResponse(
            matches
                .Select(m => new NationalRegisterMatchDto(
                    m.RegisterPersonId.Value,
                    m.GivenName,
                    m.FamilyName,
                    m.BirthDate,
                    m.Nationality,
                    m.BisNumber,
                    m.NationalRegisterNumber,
                    m.MatchScore,
                    m.MatchReason))
                .ToList());
    }
}
