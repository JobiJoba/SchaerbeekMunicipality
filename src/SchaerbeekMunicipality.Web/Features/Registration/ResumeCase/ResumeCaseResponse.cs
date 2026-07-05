using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Web.Features.Registration.ResumeCase;

public sealed record ResumeCaseResponse(Guid CaseId, string Status);
