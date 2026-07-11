using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SchaerbeekMunicipality.Api.Validation;

public static class ValidationResults
{
    public static IResult ToProblemDetails(ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }
}
