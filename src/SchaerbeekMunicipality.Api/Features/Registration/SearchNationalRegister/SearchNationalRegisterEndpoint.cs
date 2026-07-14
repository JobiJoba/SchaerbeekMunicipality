using FluentValidation;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.Registration.SearchNationalRegister;

namespace SchaerbeekMunicipality.Api.Features.Registration.SearchNationalRegister;

public static class SearchNationalRegisterEndpoint
{
    public static async Task<IResult> Handle(
        [AsParameters] SearchNationalRegisterRequest request,
        SearchNationalRegisterHandler handler,
        IValidator<SearchNationalRegisterRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        var result = await handler.Handle(request, cancellationToken);
        return Results.Ok(result);
    }
}