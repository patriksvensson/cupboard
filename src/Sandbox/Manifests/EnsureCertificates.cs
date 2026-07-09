using System.Security.Cryptography.X509Certificates;
using Cupboard;

namespace Sandbox;

public sealed class EnsureCertificates : Manifest
{
    public override void Execute(ManifestContext context)
    {
        context.Resource<Certificate>("DST Root CA X3")
            .Thumbprint("dac9024f54d8f6df94935fb1732638ca6ad77c13")
            .ValidateThumbprint()
            .Ensure(CertificateState.Absent)
            .StoreName(StoreName.Root)
            .StoreLocation(StoreLocation.LocalMachine)
            .RequireAdministrator();

        context.Resource<Certificate>("ISRG Root X1")
            .Thumbprint("cabd2a79a1076a31f21d253635cb039d4329a5e8")
            .RemoteRootThumbprint("cabd2a79a1076a31f21d253635cb039d4329a5e8") // Validate the https://x1.i.lencr.org/ root certificate
            .ValidateThumbprint()
            .FromUrl("https://x1.i.lencr.org/")
            .Ensure(CertificateState.Present)
            .StoreName(StoreName.Root)
            .StoreLocation(StoreLocation.LocalMachine)
            .RequireAdministrator();
    }
}