using System.Security.Cryptography.X509Certificates;
using Spectre.IO;

namespace Cupboard;

public static class CertificateExtensions
{
    public static IResourceBuilder<Certificate> Ensure(this IResourceBuilder<Certificate> builder, CertificateState state) =>
        builder.Configure(cert => cert.Ensure = state);

    public static IResourceBuilder<Certificate> ValidateThumbprint(this IResourceBuilder<Certificate> builder) =>
        builder.Configure(cert => cert.ValidateThumbprint = true);

    public static IResourceBuilder<Certificate> Thumbprint(this IResourceBuilder<Certificate> builder, string thumbprint) =>
        builder.Configure(cert => cert.Thumbprint = thumbprint);

    public static IResourceBuilder<Certificate> RemoteInsecure(this IResourceBuilder<Certificate> builder) =>
        builder.Configure(cert => cert.RemoteInsecure = true);

    public static IResourceBuilder<Certificate> RemoteThumbprint(this IResourceBuilder<Certificate> builder, string thumbprint) =>
        builder.Configure(cert => cert.RemoteThumbprint = thumbprint);

    public static IResourceBuilder<Certificate> RemoteRootThumbprint(this IResourceBuilder<Certificate> builder, string thumbprint) =>
        builder.Configure(cert => cert.RemoteRootThumbprint = thumbprint);

    public static IResourceBuilder<Certificate> FromUrl(this IResourceBuilder<Certificate> builder, string url) =>
        builder.Configure(cert => cert.Url = new Uri(url));

    public static IResourceBuilder<Certificate> FromFile(this IResourceBuilder<Certificate> builder, FilePath filePath) =>
        builder.Configure(cert => cert.FilePath = filePath);

    public static IResourceBuilder<Certificate> FromUrl(this IResourceBuilder<Certificate> builder, Uri url) =>
        builder.Configure(cert => cert.Url = url);

    public static IResourceBuilder<Certificate> StoreName(this IResourceBuilder<Certificate> builder, StoreName storeName) =>
        builder.Configure(cert => cert.StoreName = storeName);

    public static IResourceBuilder<Certificate> StoreLocation(this IResourceBuilder<Certificate> builder, StoreLocation storeLocation) =>
        builder.Configure(cert => cert.StoreLocation = storeLocation);

    public static IResourceBuilder<Certificate> TokenScheme(this IResourceBuilder<Certificate> builder, string scheme) =>
        builder.Configure(cert => cert.TokenScheme = scheme);

    public static IResourceBuilder<Certificate> Token(this IResourceBuilder<Certificate> builder, string token) =>
        builder.Configure(cert => cert.Token = token);

    public static IResourceBuilder<Certificate> Username(this IResourceBuilder<Certificate> builder, string username) =>
        builder.Configure(cert => cert.Username = username);

    public static IResourceBuilder<Certificate> Password(this IResourceBuilder<Certificate> builder, string password) =>
        builder.Configure(cert => cert.Password = password);
}