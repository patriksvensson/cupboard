namespace Cupboard;

internal sealed class SecurityPrincipal : ISecurityPrincipal
{
    private readonly Lazy<bool> _isAdministrator;

    bool ISecurityPrincipal.IsAdministrator => _isAdministrator.Value;

    public SecurityPrincipal()
    {
        _isAdministrator = new Lazy<bool>(() =>
            RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
                ? new WindowsPrincipal(WindowsIdentity.GetCurrent())
                    .IsInRole(WindowsBuiltInRole.Administrator)
                : Syscall.geteuid() == 0);
    }
}