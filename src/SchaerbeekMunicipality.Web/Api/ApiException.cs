using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace SchaerbeekMunicipality.Web.Api;

public sealed class ApiException : Exception
{
    public ApiException(HttpStatusCode statusCode, string message, string? title = null)
        : base(message)
    {
        StatusCode = statusCode;
        Title = title;
    }

    public HttpStatusCode StatusCode { get; }

    public string? Title { get; }

    public static async Task ThrowIfErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw await CreateAsync(response, cancellationToken);
    }

    private static async Task<Exception> CreateAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = response.StatusCode;
        var problemDetails = await TryReadProblemDetailsAsync(response, cancellationToken);

        return statusCode switch
        {
            HttpStatusCode.BadRequest when problemDetails?.Errors is { Count: > 0 } errors =>
                ToValidationException(errors),
            HttpStatusCode.Forbidden =>
                new UnauthorizedAccessException(problemDetails?.Detail ?? "Access denied."),
            HttpStatusCode.NotFound =>
                new KeyNotFoundException(problemDetails?.Detail ?? "The requested resource was not found."),
            HttpStatusCode.Conflict =>
                new InvalidOperationException(problemDetails?.Detail ?? "The request conflicted with the current state."),
            _ => new ApiException(
                statusCode,
                problemDetails?.Detail ?? $"The API request failed with status code {(int)statusCode}.",
                problemDetails?.Title),
        };
    }

    private static ValidationException ToValidationException(IDictionary<string, string[]> errors)
    {
        var failures = errors
            .SelectMany(entry => entry.Value.Select(message => new ValidationFailure(entry.Key, message)))
            .ToList();

        return new ValidationException(failures);
    }

    private static async Task<ValidationProblemDetails?> TryReadProblemDetailsAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentType?.MediaType?.Contains("json", StringComparison.OrdinalIgnoreCase) != true)
        {
            return null;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken);
        }
        catch
        {
            return null;
        }
    }
}
