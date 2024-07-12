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

        var info = UnixFileSystemInfo.GetFileSystemEntry(path.FullPath);
        info.FileAccessPermissions = chmod.ToFileAccessPermissions();
    }
}
