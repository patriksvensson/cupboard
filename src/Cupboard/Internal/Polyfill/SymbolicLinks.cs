using System;
using System.Runtime.InteropServices;
using Mono.Unix.Native;
using Spectre.IO;

namespace Cupboard.Internal
{
    internal static class SymbolicLinks
    {
        public static bool CreateSymbolicLink(Path source, Path target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source.IsRelative)
            {
                throw new InvalidOperationException("Source path cannot be relative");
            }

            if (target.IsRelative)
            {
                throw new InvalidOperationException("Source path cannot be relative");
            }

            if (source.GetType() != target.GetType())
            {
                throw new InvalidOperationException("Source path and target path must be of the same type");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var kind = source is FilePath ? Native.Win32.SymbolicLink.File : Native.Win32.SymbolicLink.Directory;
                return Native.Win32.CreateSymbolicLink(target.FullPath, source.FullPath, kind);
            }
            else
            {
                return Syscall.symlink(source.FullPath, target.FullPath) == 0;
            }
        }
    }
}
