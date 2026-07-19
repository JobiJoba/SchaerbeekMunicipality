namespace SchaerbeekMunicipality.Web.Api;

/// <summary>
///     Proxies document download routes through the Web host in development so browser
///     previews (img/iframe) can reach the API with demo-officer headers.
/// </summary>
public static class MunicipalDocumentDownloadProxy
{
    public static WebApplication MapMunicipalDocumentDownloadProxy(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

        app.MapGet(
            "/api/registration/cases/{id:guid}/documents/{documentId:guid}",
            (Guid id, Guid documentId, MunicipalDocumentDownloadForwarder forwarder, CancellationToken cancellationToken) =>
                forwarder.ForwardAsync(
                    $"/api/registration/cases/{id}/documents/{documentId}",
                    cancellationToken));

        app.MapGet(
            "/api/birth-declarations/cases/{id:guid}/documents/{documentId:guid}",
            (Guid id, Guid documentId, MunicipalDocumentDownloadForwarder forwarder, CancellationToken cancellationToken) =>
                forwarder.ForwardAsync(
                    $"/api/birth-declarations/cases/{id}/documents/{documentId}",
                    cancellationToken));

        app.MapGet(
            "/api/change-of-address/cases/{id:guid}/documents/{documentId:guid}",
            (Guid id, Guid documentId, MunicipalDocumentDownloadForwarder forwarder, CancellationToken cancellationToken) =>
                forwarder.ForwardAsync(
                    $"/api/change-of-address/cases/{id}/documents/{documentId}",
                    cancellationToken));

        app.MapGet(
            "/api/register-amendments/cases/{id:guid}/documents/{documentId:guid}",
            (Guid id, Guid documentId, MunicipalDocumentDownloadForwarder forwarder, CancellationToken cancellationToken) =>
                forwarder.ForwardAsync(
                    $"/api/register-amendments/cases/{id}/documents/{documentId}",
                    cancellationToken));

        app.MapGet(
            "/api/identity-documents/requests/{id:guid}/documents/{documentId:guid}",
            (Guid id, Guid documentId, MunicipalDocumentDownloadForwarder forwarder, CancellationToken cancellationToken) =>
                forwarder.ForwardAsync(
                    $"/api/identity-documents/requests/{id}/documents/{documentId}",
                    cancellationToken));

        return app;
    }
}
