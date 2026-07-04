using SchaerbeekMunicipality.Domain.Immigration;

namespace SchaerbeekMunicipality.Web.Features.Registration.SetResidenceCategory;

public sealed record SetResidenceCategoryRequest(ResidenceCategory Category);

public sealed record SetResidenceCategoryResponse(
    Guid CaseId,
    ResidenceCategory Category,
    bool LegalResidenceEstablished,
    string? PolicyMessage);
