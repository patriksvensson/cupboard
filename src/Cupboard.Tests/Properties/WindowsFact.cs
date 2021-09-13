using System.Runtime.InteropServices;
using Xunit;

namespace Cupboard.Tests.Unit
{
    public sealed class WindowsFact : FactAttribute
    {
        private static readonly bool _isWindows;

        static WindowsFact()
        {
            _isWindows = RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
        }

        public WindowsFact(string reason = null)
        {
            if (!_isWindows)
            {
                Skip = reason ?? "Windows test.";
            }
        }
    }
}
