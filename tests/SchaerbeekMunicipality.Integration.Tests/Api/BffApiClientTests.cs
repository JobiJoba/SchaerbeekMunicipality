using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SchaerbeekMunicipality.Application.Auth;
using SchaerbeekMunicipality.Application.Features.BirthDeclaration.AttachDocument;
using SchaerbeekMunicipality.Application.Features.Registration.OpenRegistrationCase;
using SchaerbeekMunicipality.Domain.Documents;
using SchaerbeekMunicipality.Domain.Registration;
using SchaerbeekMunicipality.Web.Api;
using SchaerbeekMunicipality.Web.Api.BirthDeclaration;
using SchaerbeekMunicipality.Web.Api.ChangeOfAddress;
using SchaerbeekMunicipality.Web.Api.Registration;

namespace SchaerbeekMunicipality.Integration.Tests.Api;

public sealed class BffApiClientTests
{
    [Fact]
    public async Task RegistrationApi_OpenAndListCases_ForwardsDemoOfficerHeaders()
    {
        await using var factory = new MunicipalAppFactory();
        using var scope = CreateClientScope(factory);

        var registrationApi = scope.ServiceProvider.GetRequiredService<IRegistrationApi>();

        var opened = await registrationApi.OpenCaseAsync(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        opened.CaseId.Should().NotBeEmpty();

        var cases = await registrationApi.ListCasesAsync();
        cases.Should().Contain(item => item.Id == opened.CaseId);
    }

    [Fact]
    public async Task RegistrationApi_AttachAndDownloadDocument_WorksOverHttp()
    {
        await using var factory = new MunicipalAppFactory();
        using var scope = CreateClientScope(factory);

        var registrationApi = scope.ServiceProvider.GetRequiredService<IRegistrationApi>();

        var opened = await registrationApi.OpenCaseAsync(
            new OpenRegistrationCaseRequest(VisitReason.FirstRegistration, null));

        await registrationApi.ClaimCaseAsync(opened.CaseId);

        await using var uploadStream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        var attached = await registrationApi.AttachDocumentAsync(
            opened.CaseId,
            DocumentType.Passport,
            uploadStream,
            "passport.pdf");

        attached.DocumentId.Should().NotBeEmpty();

        await using var downloaded = await registrationApi.DownloadDocumentAsync(
            opened.CaseId,
            attached.DocumentId);

        downloaded.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BirthDeclarationApi_AttachAndDownloadDocument_WorksOverHttp()
    {
        await using var factory = new MunicipalAppFactory();
        using var scope = CreateClientScope(factory);

        var birthDeclarationApi = scope.ServiceProvider.GetRequiredService<IBirthDeclarationApi>();

        var opened = await birthDeclarationApi.OpenCaseAsync();
        await birthDeclarationApi.ClaimCaseAsync(opened.CaseId);

        await using var uploadStream = new MemoryStream([0x25, 0x50, 0x44, 0x46]);
        AttachDocumentResponse attached = await birthDeclarationApi.AttachDocumentAsync(
            opened.CaseId,
            uploadStream,
            "medical.pdf");

        attached.DocumentId.Should().NotBeEmpty();

        await using var downloaded = await birthDeclarationApi.DownloadDocumentAsync(
            opened.CaseId,
            attached.DocumentId);

        downloaded.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RegistrationApi_InvalidRequest_ThrowsValidationException()
    {
        await using var factory = new MunicipalAppFactory();
        using var scope = CreateClientScope(factory);

        var registrationApi = scope.ServiceProvider.GetRequiredService<IRegistrationApi>();

        var act = () => registrationApi.OpenCaseAsync(
            new OpenRegistrationCaseRequest((VisitReason)999, null));

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().NotBeEmpty();
    }

    private static IServiceScope CreateClientScope(MunicipalAppFactory factory)
    {
        var services = new ServiceCollection();
        services.AddScoped<ICurrentOfficer, CurrentOfficer>();

        void ConfigureClient(IHttpClientBuilder builder) =>
            builder.ConfigureHttpClient(client => client.BaseAddress = factory.Server.BaseAddress)
                .ConfigurePrimaryHttpMessageHandler(() => factory.Server.CreateHandler());

        ConfigureClient(services.AddHttpClient<IRegistrationApi, RegistrationApiClient>());
        ConfigureClient(services.AddHttpClient<IBirthDeclarationApi, BirthDeclarationApiClient>());
        ConfigureClient(services.AddHttpClient<IChangeOfAddressApi, ChangeOfAddressApiClient>());

        return services.BuildServiceProvider().CreateScope();
    }
}
