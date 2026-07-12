using SchaerbeekMunicipality.Domain.IdentityDocuments;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Middleware;

public sealed class DomainExceptionMiddleware(RequestDelegate next, ILogger<DomainExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (MapException(ex) is (int statusCode, string title, string detail))
        {
            logger.LogDebug(ex, "Mapped domain exception to HTTP {StatusCode}", statusCode);

            await Results.Problem(
                detail: detail,
                statusCode: statusCode,
                title: title).ExecuteAsync(context);
        }
    }

    private static (int StatusCode, string Title, string Detail)? MapException(Exception ex) =>
        ex switch
        {
            KeyNotFoundException keyNotFound => (
                StatusCodes.Status404NotFound,
                "Not Found",
                keyNotFound.Message),
            UnauthorizedAccessException unauthorized => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                unauthorized.Message),
            InvalidRegistrationTransitionException invalidTransition => (
                StatusCodes.Status409Conflict,
                "Conflict",
                invalidTransition.Message),
            InvalidDocumentRequestTransitionException invalidDocumentRequest => (
                StatusCodes.Status409Conflict,
                "Conflict",
                invalidDocumentRequest.Message),
            InvalidOperationException invalidOperation when IsConflict(invalidOperation) => (
                StatusCodes.Status409Conflict,
                "Conflict",
                invalidOperation.Message),
            _ => null,
        };

    private static bool IsConflict(InvalidOperationException ex) =>
        ex.Message.Contains("lock", StringComparison.OrdinalIgnoreCase)
        || ex.Message.Contains("conflict", StringComparison.OrdinalIgnoreCase);
}

public static class DomainExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseDomainExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<DomainExceptionMiddleware>();
}
