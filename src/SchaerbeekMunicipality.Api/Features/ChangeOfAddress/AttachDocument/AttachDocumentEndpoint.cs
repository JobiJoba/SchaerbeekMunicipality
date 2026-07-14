using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.ChangeOfAddress.AttachDocument;
using SchaerbeekMunicipality.Domain.ChangeOfAddress;

namespace SchaerbeekMunicipality.Api.Features.ChangeOfAddress.AttachDocument;

public sealed class AttachDocumentForm
{
    public IFormFile? File { get; set; }
}

public sealed class AttachDocumentRequestValidator : AbstractValidator<AttachDocumentForm>
{
    public AttachDocumentRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A file is required.");

        RuleFor(x => x.File!)
            .Must(file => file.Length > 0)
            .When(x => x.File is not null)
            .WithMessage("The uploaded file cannot be empty.");
    }
}

public static class AttachDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IFormFile file,
        [FromServices] AttachDocumentHandler handler,
        [FromServices] IValidator<AttachDocumentForm> validator,
        CancellationToken cancellationToken)
    {
        var form = new AttachDocumentForm { File = file };
        var validation = await validator.ValidateAsync(form, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await handler.Handle(
                new ChangeOfAddressCaseId(id),
                file.FileName,
                stream,
                cancellationToken);

            return Results.Created(
                $"/api/change-of-address/cases/{id}/documents/{result.DocumentId}",
                result);
        }
        catch (InvalidChangeOfAddressTransitionException ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }
}