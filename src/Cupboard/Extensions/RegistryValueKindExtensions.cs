using System.Diagnostics.CodeAnalysis;

namespace Cupboard.Internal;

internal static class RegistryValueKindExtensions
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static Microsoft.Win32.RegistryValueKind ToWin32(this RegistryValueKind type)
    {
        return type switch
        {
            RegistryValueKind.None => Microsoft.Win32.RegistryValueKind.None,
            RegistryValueKind.Unknown => Microsoft.Win32.RegistryValueKind.Unknown,
            RegistryValueKind.String => Microsoft.Win32.RegistryValueKind.String,
            RegistryValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
            RegistryValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
            RegistryValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
            RegistryValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
            RegistryValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
            _ => Microsoft.Win32.RegistryValueKind.Unknown,
        };
    }
}
