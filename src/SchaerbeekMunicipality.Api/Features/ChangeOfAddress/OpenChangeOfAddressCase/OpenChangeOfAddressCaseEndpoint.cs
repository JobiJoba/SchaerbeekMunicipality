using FluentValidation;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.OpenChangeOfAddressCase;

public static class OpenChangeOfAddressCaseEndpoint
{
    public static async Task<IResult> Handle(
        OpenChangeOfAddressCaseRequest request,
        OpenChangeOfAddressCaseHandler handler,
        IValidator<OpenChangeOfAddressCaseRequest> validator,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

        try
        {
            var response = await handler.Handle(request, cancellationToken);
            return Results.Created($"/api/change-of-address/cases/{response.CaseId}", response);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}