using SchaerbeekMunicipality.Domain.Registration;

namespace SchaerbeekMunicipality.Application.Features.Registration.ResumeCase;

public sealed record ResumeCaseResponse(Guid CaseId, string Status);
