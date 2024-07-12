namespace Cupboard.Internal;

[Obsolete("Please use RegistryValueKindExtensions instead")]
internal static class RegistryKeyValueKindExtensions
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static Microsoft.Win32.RegistryValueKind ToWin32(this RegistryKeyValueKind type)
    {
        return type switch
        {
            RegistryKeyValueKind.None => Microsoft.Win32.RegistryValueKind.None,
            RegistryKeyValueKind.Unknown => Microsoft.Win32.RegistryValueKind.Unknown,
            RegistryKeyValueKind.String => Microsoft.Win32.RegistryValueKind.String,
            RegistryKeyValueKind.ExpandString => Microsoft.Win32.RegistryValueKind.ExpandString,
            RegistryKeyValueKind.Binary => Microsoft.Win32.RegistryValueKind.Binary,
            RegistryKeyValueKind.DWord => Microsoft.Win32.RegistryValueKind.DWord,
            RegistryKeyValueKind.MultiString => Microsoft.Win32.RegistryValueKind.MultiString,
            RegistryKeyValueKind.QWord => Microsoft.Win32.RegistryValueKind.QWord,
            _ => Microsoft.Win32.RegistryValueKind.Unknown,
        };
    }
}
