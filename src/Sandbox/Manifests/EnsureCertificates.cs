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
            .ValidateThumbprint()
            .FromUrl("http://x1.i.lencr.org/") // use HTTP since HTTPS uses the ISRG Root X1 in the chain 😮
            .Ensure(CertificateState.Present)
            .StoreName(StoreName.Root)
            .StoreLocation(StoreLocation.LocalMachine)
            .RequireAdministrator();
    }
}