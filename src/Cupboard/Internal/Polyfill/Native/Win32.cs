using System.Runtime.InteropServices;

namespace Cupboard.Internal
{
    internal static partial class Native
    {
        public static class Win32
        {
            public enum SymbolicLink
            {
                File = 0,
                Directory = 1,
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
        }
    }
}
