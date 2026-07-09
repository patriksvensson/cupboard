using System.Security.Cryptography.X509Certificates;

namespace Cupboard.Tests.Unit.IO;

public class HttpClientTests
{
    [Fact]
    public async Task Should_Validate_Remote_Certificate()
    {
        // Given
        var handler = new HttpClientHandler
        {
            // Likely to become an issue in 2035 when the root certificate expires
            ServerCertificateCustomValidationCallback = (_, _, chain, _) =>
                chain.ChainElements[^1].Certificate.Thumbprint.Equals(
                    "cabd2a79a1076a31f21d253635cb039d4329a5e8", StringComparison.OrdinalIgnoreCase),
        };
        var httpClient = new HttpClient(handler);
        var url = new Uri("https://x1.i.lencr.org/");

        // When
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(httpRequest);
        var certificateBytes = await response.Content.ReadAsByteArrayAsync();
        var certificate = new X509Certificate2(certificateBytes);

        // Then
        certificate.ShouldNotBeNull();
        certificate.Verify();
    }

    [Fact]
    public async Task Should_Not_Validate_Remote_Certificate()
    {
        // Given
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                chain.ChainElements[^1].Certificate.Thumbprint.Equals(
                    "NotValid", StringComparison.OrdinalIgnoreCase),
        };
        var httpClient = new HttpClient(handler);
        var url = new Uri("https://x1.i.lencr.org/");

        // When
        var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
        var action = async () => await httpClient.SendAsync(httpRequest);

        // Then
        await action.ShouldThrowAsync<HttpRequestException>();
    }
}