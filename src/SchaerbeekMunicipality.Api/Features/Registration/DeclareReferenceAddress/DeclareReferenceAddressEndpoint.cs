using SchaerbeekMunicipality.Application.Features.Registration.DeclareReferenceAddress;
using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Api.Features.Registration.DeclareReferenceAddress;

public static class DeclareReferenceAddressEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        DeclareReferenceAddressHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await handler.Handle(new RegistrationCaseId(id), cancellationToken);
            return Results.Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
    }
}
