using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.PersonFile.SearchPersonFile;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Api.Features.PersonFile.SearchPersonFile;

public static class SearchPersonFileEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] SearchNationalRegisterRequest request,
        SearchPersonFileHandler handler,
        IValidator<SearchNationalRegisterRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        var result = await handler.Handle(request, cancellationToken);
        return Results.Ok(result);
    }
}