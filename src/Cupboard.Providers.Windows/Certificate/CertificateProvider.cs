using System.Security.Cryptography.X509Certificates;
using Spectre.IO;

namespace Cupboard;

public class CertificateProvider : AsyncResourceProvider<Certificate>
{
    private readonly ICupboardFileSystem _fileSystem;
    private readonly ICupboardEnvironment _environment;
    private readonly ICupboardLogger _logger;
    private readonly HttpClient _http;

    public CertificateProvider(
        ICupboardFileSystem fileSystem,
        ICupboardEnvironment environment,
        ICupboardLogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _http = new HttpClient();
    }

    public override Certificate Create(string name)
    {
        return new Certificate(name);
    }

    public override async Task<ResourceState> RunAsync(IExecutionContext context, Certificate resource)
    {
        return resource.Ensure switch
        {
            CertificateState.Present => await EnsurePresent(resource, context).ConfigureAwait(false),
            CertificateState.Absent => await EnsureAbsent(resource, context).ConfigureAwait(false),
            _ => ResourceState.Error,
        };
    }

    private static string ToStoreNameString(StoreName storeName)
    {
        return storeName switch
        {
            StoreName.CertificateAuthority => "Intermediate Certification Authorities",
            StoreName.AddressBook => "Other People",
            StoreName.AuthRoot => "Third-Party Root Certification Authorities",
            StoreName.Disallowed => "Untrusted Certificates",
            StoreName.My => "Personal Certificates",
            StoreName.Root => "Trusted Root Certification Authorities",
            StoreName.TrustedPeople => "Trusted People",
            StoreName.TrustedPublisher => "Trusted Publisher",
            _ => "Non existing",
        };
    }

    private static string ToStoreLocationString(StoreLocation storeLocation)
    {
        return storeLocation switch
        {
            StoreLocation.LocalMachine => "Local Machine",
            StoreLocation.CurrentUser => "Current User",
            _ => "Non existing",
        };
    }

    private X509Certificate2? FindCertificateInStore(Certificate resource, X509Store store)
    {
        X509Certificate2? certificate = null;
        foreach (var storeCertificate in store.Certificates)
        {
            if (storeCertificate.Thumbprint.Equals(resource.Thumbprint, StringComparison.InvariantCultureIgnoreCase) is false
                && storeCertificate.FriendlyName.Contains(resource.Name, StringComparison.InvariantCultureIgnoreCase) is false)
            {
                continue;
            }

            _logger.Debug("Certificate found in store based on thumbprint or name");
            certificate = storeCertificate;
            break;
        }

        return certificate;
    }

    private async Task<X509Certificate2?> RetrieveCertificate(Certificate resource, X509Store store)
    {
        X509Certificate2? certificate = null;

        if (resource.FilePath is not null && resource.Url is null)
        {
            var filePath = resource.FilePath.MakeAbsolute(_environment);
            if (_fileSystem.Exist(filePath) is false)
            {
                _logger.Error("Certificate file does not exist");
            }

            certificate = new X509Certificate2(filePath.FullPath);
        }

        if (resource.Url is not null && resource.FilePath is null)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, resource.Url);
            using var response = await _http.SendAsync(request).ConfigureAwait(false);
            var certificateBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            certificate = new X509Certificate2(certificateBytes);
        }

        certificate ??= FindCertificateInStore(resource, store);

        return certificate;
    }

    private async Task<ResourceState> EnsureAbsent(Certificate resource, IExecutionContext context)
    {
        _logger.Debug($"Opening Certificate Store: [yellow]{ToStoreNameString(resource.StoreName)}[/] Location: [yellow]{ToStoreLocationString(resource.StoreLocation)}[/]");
        using var store = new X509Store(resource.StoreName, resource.StoreLocation);
        store.Open(OpenFlags.ReadWrite);
        var certificate = await RetrieveCertificate(resource, store).ConfigureAwait(false);

        if (certificate is null)
        {
            return ResourceState.Unchanged;
        }

        if (resource.ValidateThumbprint && resource.Thumbprint.Equals(certificate.Thumbprint, StringComparison.InvariantCultureIgnoreCase) is false)
        {
            _logger.Error($"Certificate thumbprint does not match. The provided thumbprint '{resource.Thumbprint}' is not equal to the thumbprint inside the certificate '{certificate.Thumbprint}'");
            return ResourceState.Error;
        }

        if (store.Certificates.Contains(certificate) is false)
        {
            return ResourceState.Unchanged;
        }

        if (context.DryRun is false)
        {
            store.Remove(certificate);
        }

        return ResourceState.Changed;
    }

    private async Task<ResourceState> EnsurePresent(Certificate resource, IExecutionContext context)
    {
        _logger.Debug($"Opening Certificate Store: [yellow]{ToStoreNameString(resource.StoreName)}[/] Location: [yellow]{ToStoreLocationString(resource.StoreLocation)}[/]");
        using var store = new X509Store(resource.StoreName, resource.StoreLocation);
        store.Open(OpenFlags.ReadWrite);
        var certificate = await RetrieveCertificate(resource, store).ConfigureAwait(false);

        if (certificate is null)
        {
            _logger.Error("No certificate specified");
            return ResourceState.Error;
        }

        if (resource.ValidateThumbprint && resource.Thumbprint.Equals(certificate.Thumbprint, StringComparison.InvariantCultureIgnoreCase) is false)
        {
            _logger.Error($"Certificate thumbprint does not match. The provided thumbprint '{resource.Thumbprint}' is not equal to the thumbprint inside the certificate '{certificate.Thumbprint}'");
            return ResourceState.Error;
        }

        if (store.Certificates.Contains(certificate))
        {
            return ResourceState.Unchanged;
        }

        if (context.DryRun is false)
        {
            store.Add(certificate);
        }

        return ResourceState.Changed;
    }
}