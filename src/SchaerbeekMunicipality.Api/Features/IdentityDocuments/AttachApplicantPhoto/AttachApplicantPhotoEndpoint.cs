using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchaerbeekMunicipality.Api.Validation;
using SchaerbeekMunicipality.Application.Features.IdentityDocuments.AttachApplicantPhoto;
using SchaerbeekMunicipality.Domain.IdentityDocuments;

namespace SchaerbeekMunicipality.Api.Features.IdentityDocuments.AttachApplicantPhoto;

public sealed class AttachApplicantPhotoForm
{
    public IFormFile? File { get; set; }
}

public sealed class AttachApplicantPhotoRequestValidator : AbstractValidator<AttachApplicantPhotoForm>
{
    public AttachApplicantPhotoRequestValidator()
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

public static class AttachApplicantPhotoEndpoint
{
    public static async Task<IResult> Handle(
        Guid id,
        IFormFile file,
        [FromServices] AttachApplicantPhotoHandler handler,
        [FromServices] IValidator<AttachApplicantPhotoForm> validator,
        CancellationToken cancellationToken)
    {
        var form = new AttachApplicantPhotoForm { File = file };
        var validation = await validator.ValidateAsync(form, cancellationToken);
        if (!validation.IsValid) return ValidationResults.ToProblemDetails(validation);

        await using var stream = file.OpenReadStream();
        var result = await handler.Handle(
            DocumentRequestCaseId.From(id),
            file.FileName,
            stream,
            cancellationToken);

        return Results.Created(
            $"/api/identity-documents/requests/{id}/documents/{result.DocumentId}",
            result);
    }
}