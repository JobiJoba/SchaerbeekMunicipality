using FluentValidation;
using SchaerbeekMunicipality.Application.Features.RegisterAmendment.OpenRegisterAmendmentCase;
using SchaerbeekMunicipality.Domain.RegisterAmendment;

namespace SchaerbeekMunicipality.Api.Features.RegisterAmendment.OpenRegisterAmendmentCase;

public static class OpenRegisterAmendmentCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenRegisterAmendmentCaseRequest request,
        OpenRegisterAmendmentCaseHandler handler,
        IValidator<OpenRegisterAmendmentCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        try
        {
            var response = await handler.Handle(request, cancellationToken);
            return Results.Created($"/api/register-amendments/cases/{response.CaseId}", response);
        }
        catch (InvalidRegisterAmendmentTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}
