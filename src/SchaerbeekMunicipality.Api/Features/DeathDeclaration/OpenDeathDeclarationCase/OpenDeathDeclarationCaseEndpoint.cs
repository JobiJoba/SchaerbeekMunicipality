using FluentValidation;
using SchaerbeekMunicipality.Application.Features.DeathDeclaration.OpenDeathDeclarationCase;
using SchaerbeekMunicipality.Domain.DeathDeclaration;

namespace SchaerbeekMunicipality.Api.Features.DeathDeclaration.OpenDeathDeclarationCase;

public static class OpenDeathDeclarationCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenDeathDeclarationCaseRequest request,
        OpenDeathDeclarationCaseHandler handler,
        IValidator<OpenDeathDeclarationCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        try
        {
            var response = await handler.Handle(request, cancellationToken);
            return Results.Created($"/api/death-declarations/cases/{response.CaseId}", response);
        }
        catch (InvalidDeathDeclarationTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
