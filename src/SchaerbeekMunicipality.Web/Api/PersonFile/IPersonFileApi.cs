using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.PersonFile.GetPersonFile;
using SchaerbeekMunicipality.Application.Features.PersonFile.ListPersonCases;
using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Web.Api.PersonFile;

public interface IPersonFileApi
{
    Task<GetPersonFileResponse> GetPersonFileAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<GetPersonFileResponse> GetPersonFileByNationalRegisterNumberAsync(
        string nationalRegisterNumber,
        CancellationToken cancellationToken = default);

    Task<SearchPersonFileResponse> SearchPersonFileAsync(
        SearchNationalRegisterRequest request,
        CancellationToken cancellationToken = default);

    Task<ListPersonCasesResponse> ListPersonCasesAsync(
        Guid personId,
        CancellationToken cancellationToken = default);
}
