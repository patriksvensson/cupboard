using System.Runtime.InteropServices;
using System.Security.Principal;
using Mono.Unix.Native;

namespace Cupboard.Internal;

internal sealed class SecurityPrincipal : ISecurityPrincipal
{
    public bool IsAdministrator()
    {
        return RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ?
            new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator) :
            Syscall.geteuid() == 0;
    }
}