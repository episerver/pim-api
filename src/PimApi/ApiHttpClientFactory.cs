using EPiServer.Turnstile.Contracts.Hmac;
using System.Net.Http;

namespace PimApi;

/// <summary>Creates HttpClient instances configured with property authentication for PIM access.</summary>
public sealed class ApiHttpClientFactory(ConnectionInformation connectionInformation)
{
    public HttpClient Create()
    {
        if (connectionInformation?.PimUrlBase is null)
        {
            throw new ArgumentNullException(nameof(connectionInformation.PimUrlBase));
        }

        var secretKeyAsBytes = Convert.FromBase64String(connectionInformation.AppSecret);
        var client = HttpClientFactory.Create(
            new HttpClientHandler(),
            new HmacMessageHandler(
                new DefaultHmacDeclarationFactory(new Sha256HmacAlgorithm(secretKeyAsBytes)),
                connectionInformation.AppKey
            )
        );
        client.BaseAddress = new Uri(connectionInformation.PimUrlBase);

        return client;
    }
}
