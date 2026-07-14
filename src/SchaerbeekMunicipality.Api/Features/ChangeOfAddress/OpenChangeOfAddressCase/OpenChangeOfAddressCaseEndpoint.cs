using FluentValidation;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.OpenChangeOfAddressCase;

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

        var response = await handler.Handle(request, cancellationToken);
        return Results.Created($"/api/change-of-address/cases/{response.CaseId}", response);
    }
}