using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.Police;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Auth;
using SchaerbeekMunicipality.Web.Features.Registration.ClaimRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Web.Features.Registration.RecordPoliceResult;

namespace SchaerbeekMunicipality.Integration.Tests.Features.Registration;

internal static class RegistrationTestHelpers
{
    internal static async Task<RegistrationCaseId> OpenAndClaimCaseAsync(
        IServiceProvider services,
        VisitReason visitReason = VisitReason.FirstRegistration,
        CancellationToken cancellationToken = default)
    {
        var openHandler = services.GetRequiredService<OpenRegistrationCaseHandler>();
        var claimHandler = services.GetRequiredService<ClaimRegistrationCaseHandler>();

        var opened = await openHandler.Handle(
            new OpenRegistrationCaseRequest(visitReason, null),
            cancellationToken);

        var caseId = new RegistrationCaseId(opened.CaseId);
        await claimHandler.Handle(caseId, cancellationToken);
        return caseId;
    }

    internal static async Task RecordPoliceResultAsync(
        IServiceProvider services,
        Guid requestId,
        PoliceVerificationResult result,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        SetRole(services, OfficerRole.PoliceClerk);

        await services.GetRequiredService<RecordPoliceResultHandler>().Handle(
            PoliceVerificationRequestId.From(requestId),
            new RecordPoliceResultRequest(result, notes),
            cancellationToken);

        SetRole(services, OfficerRole.PopulationOfficer);
    }

    internal static void SetRole(IServiceProvider services, OfficerRole role)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SetRole(role);
    }

    internal static void SelectPopulationOfficer(IServiceProvider services, bool secondary = false)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SelectOfficer(
            secondary
                ? CurrentOfficer.SecondaryPopulationOfficerId
                : CurrentOfficer.PopulationOfficerId);
    }

    internal static void ImpersonatePopulationOfficer(
        IServiceProvider services,
        Guid officerId,
        string displayName)
    {
        var officer = services.GetRequiredService<ICurrentOfficer>();
        officer.SelectOfficer(officerId);
    }
}
