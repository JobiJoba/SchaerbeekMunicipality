using FluentValidation;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Validation;

namespace SchaerbeekMunicipality.Web.Features.Registration.AttachDocument;

public sealed class AttachDocumentRequestValidator : AbstractValidator<AttachDocumentForm>
{
    public AttachDocumentRequestValidator()
    {
        RuleFor(x => x.DocumentType)
            .IsInEnum()
            .WithMessage("A valid document type is required.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A file is required.");

        RuleFor(x => x.File!)
            .Must(file => file.Length > 0)
            .When(x => x.File is not null)
            .WithMessage("The uploaded file cannot be empty.");
    }
}

public sealed class AttachDocumentForm
{
    public DocumentType DocumentType { get; set; }

    public IFormFile? File { get; set; }
}

public static class AttachDocumentEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IFormFile file,
        DocumentType documentType,
        AttachDocumentHandler handler,
        IValidator<AttachDocumentForm> validator,
        CancellationToken cancellationToken)
    {
        var form = new AttachDocumentForm { DocumentType = documentType, File = file };
        var validation = await validator.ValidateAsync(form, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationResults.ToProblemDetails(validation);
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await handler.Handle(
                new RegistrationCaseId(id),
                documentType,
                file.FileName,
                stream,
                cancellationToken);

            return Results.Created(
                $"/api/registration/cases/{id}/documents/{result.DocumentId}",
                result);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound();
        }
        catch (InvalidRegistrationTransitionException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid registration transition");
        }
    }
}
