using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Domain.BirthDeclaration;
using SchaerbeekMunicipality.Domain.NationalRegister;
using SchaerbeekMunicipality.Infrastructure.Persistence;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.ConfirmBirthDeclaration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.LinkParent;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.OpenBirthDeclarationCase;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.RecordChildDetails;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.SetDeclarationHousehold;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Integration.Tests.Features.Registration;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.AttachDocument;

namespace SchaerbeekMunicipality.Integration.Tests.Features.BirthDeclaration;

public sealed class BirthDeclarationTests
{
    [Fact]
    public async Task ReceptionBirthDeclarationReason_OpensBirthDeclarationCase_NotRegistrationCase()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();

        RegistrationTestHelpers.SetRole(scope.ServiceProvider, OfficerRole.ReceptionOfficer);

        var birthHandler = scope.ServiceProvider.GetRequiredService<OpenBirthDeclarationCaseHandler>();
        var birthResult = await birthHandler.Handle(CancellationToken.None);

        var registrationHandler = scope.ServiceProvider.GetRequiredService<OpenRegistrationCaseHandler>();
        var registrationResult = await registrationHandler.Handle(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null),
            CancellationToken.None);

        var birthRepo = scope.ServiceProvider.GetRequiredService<IBirthDeclarationCaseRepository>();
        var registrationRepo = scope.ServiceProvider.GetRequiredService<IRegistrationCaseRepository>();

        var birthCase = await birthRepo.GetByIdAsync(new BirthDeclarationCaseId(birthResult.CaseId), CancellationToken.None);
        var registrationCase = await registrationRepo.GetByIdAsync(new RegistrationCaseId(registrationResult.CaseId), CancellationToken.None);

        birthCase.Should().NotBeNull();
        registrationCase.Should().NotBeNull();
        birthCase!.Status.Should().Be(BirthDeclarationCaseStatus.Intake);
        registrationCase!.VisitReason.Should().Be(VisitReason.FirstRegistration);
    }

    [Fact]
    public async Task HappyPath_OpenRecordLinkAttachConfirm()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await BirthDeclarationTestHelpers.OpenAndClaimCaseAsync(services);
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        await services.GetRequiredService<RecordChildDetailsHandler>().Handle(
            caseId,
            new RecordChildDetailsRequest("Amélie", "Dupont", NewbornSex.Female, yesterday, null, "CHU Saint-Pierre"),
            CancellationToken.None);

        var registerRepo = services.GetRequiredService<INationalRegisterRepository>();
        var jeanDupont = await registerRepo.GetByIdAsync(NationalRegisterSeeder.JeanDupontId, CancellationToken.None);
        jeanDupont.Should().NotBeNull();

        await services.GetRequiredService<LinkParentHandler>().Handle(
            caseId,
            new LinkParentRequest(jeanDupont!.Id.Value, ParentRole.Father),
            CancellationToken.None);

        await using var pdf = new MemoryStream("%PDF-1.4 birth"u8.ToArray());
        await services.GetRequiredService<AttachDocumentHandler>().Handle(
            caseId,
            "medical.pdf",
            pdf,
            CancellationToken.None);

        await services.GetRequiredService<SetDeclarationHouseholdHandler>().Handle(
            caseId,
            new SetDeclarationHouseholdRequest("Chaussée de Louvain", "10", null, "1030", "Schaerbeek"),
            CancellationToken.None);

        var confirmed = await services.GetRequiredService<ConfirmBirthDeclarationHandler>().Handle(
            caseId,
            CancellationToken.None);

        confirmed.Status.Should().Be(nameof(BirthDeclarationCaseStatus.Confirmed));
        confirmed.NationalRegisterNumber.Should().NotBeNullOrWhiteSpace();

        var birthRepo = services.GetRequiredService<IBirthDeclarationCaseRepository>();
        var birthCase = await birthRepo.GetByIdAsync(caseId, CancellationToken.None);
        birthCase!.Status.Should().Be(BirthDeclarationCaseStatus.Confirmed);
    }

    [Fact]
    public async Task Confirm_WithDuplicateNrAssignment_ThrowsConflict()
    {
        await using var factory = new MunicipalAppFactory();
        await using var scope = factory.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var caseId = await BirthDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services);
        var confirmHandler = services.GetRequiredService<ConfirmBirthDeclarationHandler>();

        await confirmHandler.Handle(caseId, CancellationToken.None);

        var secondCaseId = await BirthDeclarationTestHelpers.PrepareCaseReadyForConfirmationAsync(services);
        var act = () => confirmHandler.Handle(secondCaseId, CancellationToken.None);

        await act.Should().ThrowAsync<NationalRegisterConflictException>();
    }
}
