using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using SchaerbeekMunicipality.Integration.Tests;
using SchaerbeekMunicipality.Web;
using SchaerbeekMunicipality.Web.Api;
using SchaerbeekMunicipality.Web.Components;
using SchaerbeekMunicipality.Web.DesignSystem.Components.Dialogs;

namespace SchaerbeekMunicipality.E2E.Tests.Infrastructure;

internal sealed class MunicipalWebHost : IAsyncDisposable
{
    private WebApplication? _app;

    public Uri BaseUri { get; private set; } = null!;

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }

    public async Task StartAsync(MunicipalAppFactory apiFactory, CancellationToken cancellationToken = default)
    {
        var port = MunicipalWebAppFactory.GetFreeTcpPort();
        var webBinRoot = Path.GetDirectoryName(typeof(WebProgram).Assembly.Location)!;

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = Environments.Development,
            ContentRootPath = webBinRoot
        });

        builder.WebHost.UseSetting(WebHostDefaults.ServerUrlsKey, $"http://127.0.0.1:{port}");
        builder.WebHost.UseStaticWebAssets();

        builder.Services.AddSingleton<IMunicipalApiBridge>(
            new MunicipalApiBridge(
                apiFactory.Server.BaseAddress,
                apiFactory.Server.CreateHandler()));

        builder.AddServiceDefaults();
        builder.Services.AddWebPresentation();
        builder.Services.AddMunicipalApiClients(builder.Environment);
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<IAppDialogService, AppDialogService>();

        _app = builder.Build();
        _app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        _app.UseAntiforgery();
        _app.MapStaticAssets();
        _app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        _app.UseHealthCheckApiKeyProtection();
        _app.MapDefaultEndpoints();

        await _app.StartAsync(cancellationToken);
        BaseUri = new Uri($"http://127.0.0.1:{port}/");
    }
}