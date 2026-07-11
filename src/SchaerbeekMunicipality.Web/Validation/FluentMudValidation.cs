using FluentValidation;

namespace SchaerbeekMunicipality.Web.Validation;

public static class FluentMudValidation
{
    public static Func<object, string, Task<IEnumerable<string>>> ToMudValidateValue<TRequest, TForm>(
        this IValidator<TRequest> validator,
        Func<TForm, TRequest> map)
    {
        return async (model, propertyName) =>
        {
            if (model is not TForm form)
            {
                return [];
            }

            return await ValidatePropertyAsync(validator, map(form), propertyName);
        };
    }

    private static async Task<IEnumerable<string>> ValidatePropertyAsync<T>(
        IValidator<T> validator,
        T instance,
        string propertyName)
    {
        var context = ValidationContext<T>.CreateWithOptions(
            instance,
            options => options.IncludeProperties(propertyName));

        var result = await validator.ValidateAsync(context);
        return result.IsValid
            ? []
            : result.Errors.Select(error => error.ErrorMessage);
    }
}
