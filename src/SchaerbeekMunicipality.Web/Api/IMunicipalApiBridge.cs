namespace SchaerbeekMunicipality.Web.Api;

/// <summary>
///     Routes Web HttpClients to an in-process API host (used by E2E and integration tests).
/// </summary>
public interface IMunicipalApiBridge
{
    Uri BaseAddress { get; }

    HttpMessageHandler CreateHandler();
}