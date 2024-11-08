namespace Cupboard;

[PublicAPI]
public static class PathExtensions
{
    public static void SetPermissions(this Path path, Chmod chmod)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return;
        }

        File.SetUnixFileMode(path.FullPath, chmod.ToFileAccessPermissions());
    }
}
