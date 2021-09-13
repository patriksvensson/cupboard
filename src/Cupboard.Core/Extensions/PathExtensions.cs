using System;
using System.Runtime.InteropServices;
using Mono.Unix;
using Spectre.IO;

namespace Cupboard
{
    public static class PathExtensions
    {
        public static void SetPermissions(this Path path, Chmod chmod)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return;
            }

            var info = UnixFileSystemInfo.GetFileSystemEntry(path.FullPath);
            info.FileAccessPermissions = chmod.ToFileAccessPermissions();
        }
    }
}
