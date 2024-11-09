using System.Security.Cryptography.X509Certificates;
using Spectre.IO;

namespace Cupboard;

public sealed class Certificate : Resource
{
    public Uri? Url { get; set; }

    public FilePath? FilePath { get; set; }

    public StoreLocation StoreLocation { get; set; }

    public StoreName StoreName { get; set; }

    public CertificateState Ensure { get; set; }

    public string? Thumbprint { get; set; }

    public bool ValidateThumbprint { get; set; }

    public bool RemoteInsecure { get; set; }

    public string? RemoteThumbprint { get; set; }

    public string? RemoteRootThumbprint { get; set; }

    public Certificate(string name)
        : base(name)
    {
    }
}