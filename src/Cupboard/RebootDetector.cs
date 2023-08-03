using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Cupboard;

internal sealed class RebootDetector : IRebootDetector
{
    private readonly IWindowsRegistry _registry;

    private readonly List<RegistryPath> _checkForExistence = new()
    {
        @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending",
        @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootInProgress",
        @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired",
        @"HKLM:\Software\Microsoft\Windows\CurrentVersion\Component Based Servicing\PackagesPending",
        @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\PostRebootReporting",
        @"HKLM:\SOFTWARE\Microsoft\ServerManager\CurrentRebootAttemps",
    };

    private readonly List<(RegistryPath Path, string Value)> _checkForValue = new()
    {
        (@"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "DVDRebootSignal"),
        (@"HKLM:\SYSTEM\CurrentControlSet\Services\Netlogon", "JoinDomain"),
        (@"HKLM:\SYSTEM\CurrentControlSet\Services\Netlogon", "AvoidSpnSet"),
        (@"HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager", "PendingFileRenameOperations"),
        (@"HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager", "PendingFileRenameOperations2"),
    };

    public RebootDetector(IWindowsRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public bool HasPendingReboot()
    {
        if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return false;
        }

        // Check registry keys that must exist
        if (_checkForExistence.Any(path => KeyExist(path)))
        {
            return true;
        }

        // Check registry values that must exist
        if (_checkForValue.Any(p => KeyHasValue(p.Path, p.Value)))
        {
            return true;
        }

        // Check if Windows Update is waiting on updating any executables
        var wu = _registry.GetKey(@"HKLM:\SOFTWARE\Microsoft\Updates", writable: false);
        if (wu?.GetValue("UpdateExeVolatile") is int updateExeVolatile && updateExeVolatile != 0)
        {
            return true;
        }

        // Check for pending Windows updates
        wu = _registry.GetKey(@"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Services\Pending", writable: false);
        return wu?.GetValueCount() > 0;
    }

    private bool KeyExist(RegistryPath path)
    {
        var key = _registry.GetKey(path, writable: false);
        return key != null;
    }

    private bool KeyHasValue(RegistryPath path, string value)
    {
        var key = _registry.GetKey(path, writable: false);
        if (key != null)
        {
            return key.ValueExists(value);
        }

        return false;
    }
}
